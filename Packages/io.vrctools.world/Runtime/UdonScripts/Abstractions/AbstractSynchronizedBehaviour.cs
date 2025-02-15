using VRCTools.Event;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common;

namespace VRCTools.World.Abstractions {
  /// <summary>
  ///   Provides a simple basis for synchronized Udon behaviours which respect network congestion as well as failure
  ///   conditions.
  ///   Implementations of this base class will automatically synchronize their state when ownership changes players join
  ///   as well as leave.
  ///   Additionally, synchronizations will be retried every <see cref="synchronizationBackoff" /> seconds if the network
  ///   is congested or synchronization fails for any reason.
  /// </summary>
  public abstract class AbstractSynchronizedBehaviour : AbstractEventEmitter {
    /// <summary>
    ///   Defines the amount of time between synchronization retries.
    ///   When synchronization fails or is impossible due to the network being clogged, synchronization will be skipped
    ///   until the backoff period has passed.
    /// </summary>
    public float synchronizationBackoff = 2;

    /// <summary>
    ///   Defines whether synchronization debug information shall be written to the game log.
    /// </summary>
    public bool synchronizationDebugging;

    /// <summary>
    ///   Indicates whether a secondary synchronization (e.g. one which has been requested while another was still
    ///   processing) has been requested.
    /// </summary>
    private bool _secondarySynchronizationPending;

    /// <summary>
    ///   Stores the time remaining before another synchronization will be attempted.
    /// </summary>
    private float _synchronizationTimer;

    /// <summary>
    ///   Provides a simple locking mechanism which prevents multiple synchronizations from being performed at the same
    ///   time.
    /// </summary>
    private bool _synchronizing;

    /// <summary>
    ///   Evaluates whether synchronization is still ongoing within this behaviour.
    ///   This property may be checked if ownership transfer requests are used within the implementing script in order
    ///   to prevent ownership transfer while prior state synchronization remains active.
    /// </summary>
    protected bool HasOngoingSynchronization => this._synchronizing || this._synchronizationTimer > 0 ||
                                                this._secondarySynchronizationPending;

    // defaults to zero since not all synchronized behaviours will want to emit their own events - this comes at no extra
    // cost for scripts which do not make use of this functionality
    public override int EventCount => 0;

    protected virtual void Start() {
      if (this.synchronizationBackoff <= 0) {
        this._LogError(
          $"Specified invalid synchronization backoff of {this.synchronizationBackoff} - Defaulting to 1 second");
        this.synchronizationBackoff = 1;
      }

      // if we are the object owner in a freshly initialized instance, request synchronization immediately to enforce
      // the object default state on any already connected clients
      if (Networking.IsOwner(this.gameObject)) this._TrySynchronize();
    }

    protected virtual void Update() {
      if (!Networking.IsOwner(this.gameObject)) return;

      if (this._synchronizing || this._synchronizationTimer <= 0) return;

      this._synchronizationTimer -= Time.deltaTime;
      if (this._synchronizationTimer >= 0) return;

      this._DoSynchronize();
    }

    /// <summary>
    ///   Ensures that the parent game object is owned by the local player.
    ///   Note that this method may have additional implications if ownership requests are enabled (e.g. the caller must
    ///   ensure that it actually gains ownership prior to performing other actions).
    /// </summary>
    protected void _EnsureOwnership() {
      if (!Networking.IsOwner(this.gameObject)) Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
    }

    /// <summary>
    ///   Attempts to synchronize the current component state via the network.
    ///   This function is expected to be invoked on the game object owner. If invoked on another client, the call will
    ///   be ignored.
    ///   This method effectively encapsulates <see cref="UdonSharpBehaviour.RequestSerialization" /> in such a way that
    ///   synchronization is retried when the network is clogged or synchronization is otherwise impossible.
    /// </summary>
    protected void _TrySynchronize() {
      if (!Networking.IsOwner(this.gameObject)) {
        this._LogWarn("Requested synchronization while not current game object owner - Ignored");
        return;
      }

      if (this._synchronizing || this._synchronizationTimer > 0) {
        this._LogInfo(
          "Requested synchronization while prior synchronization is still pending - Scheduling secondary synchronization");
        this._secondarySynchronizationPending = true;
        return;
      }

      if (this._synchronizationTimer > 0) {
        this._LogWarn("Requested synchronization while within backoff period - Ignored");
        return;
      }

      this._DoSynchronize();
    }

    /// <summary>
    ///   Schedules a synchronization retry using the configured period.
    /// </summary>
    private void _ScheduleSynchronizationRetry() {
      this._synchronizing = false;
      this._synchronizationTimer = this.synchronizationBackoff;

      if (this._secondarySynchronizationPending) {
        this._LogWarn("Ignoring secondary synchronization within backoff period");
        this._secondarySynchronizationPending = false;
      }
    }

    /// <summary>
    ///   Actually performs the synchronization step.
    ///   If the network indicates that it is currently congested, a retry will be scheduled instead.
    /// </summary>
    private void _DoSynchronize() {
      if (Networking.IsClogged) {
        this._LogWarn("Networking is currently clogged - Scheduling retry");
        this._ScheduleSynchronizationRetry();
        return;
      }

      this._LogInfo("Attempting to synchronize");
      this._synchronizing = true;
      this.RequestSerialization();
    }

    public override void OnPlayerJoined(VRCPlayerApi player) {
      if (!Networking.IsOwner(this.gameObject)) return;

      this._LogInfo("Player joined - Scheduling synchronization");
      this._TrySynchronize();
    }

    public override void OnPlayerLeft(VRCPlayerApi player) {
      if (!Networking.IsOwner(this.gameObject)) return;

      this._LogInfo("Player left - Scheduling synchronization");
      this._TrySynchronize();
    }

    public override void OnOwnershipTransferred(VRCPlayerApi player) {
      // when ownership is gained, we'll ensure synchronization to mitigate any divergence in state which may have
      // occurred in the meantime
      if (player.isLocal) {
        this._LogInfo("Gained ownership - Requesting synchronization");
        this._TrySynchronize();
        return;
      }

      // when ownership is lost, all in-flight synchronization will be cancelled as we no longer have the capability to
      // update the state - this usually happens when somebody else mutates the object state
      this._LogInfo("Lost ownership - Cancelling any pending synchronization");
      this._synchronizing = false;
      this._synchronizationTimer = -1;
      this._secondarySynchronizationPending = false;
    }

    public override void OnPostSerialization(SerializationResult result) {
      // reset the synchronization lock so that implementors are able to request synchronization once again
      this._synchronizing = false;

      // if the synchronization was successful, ensure that all remaining synchronization timers are reset to a safe
      // value (this has typically already happened but we're playing it safe here)
      if (result.success) {
        this._LogInfo("Synchronizing successful");
        this._synchronizationTimer = -1;

        if (this._secondarySynchronizationPending) {
          this._LogInfo("Performing secondary synchronization");
          this._secondarySynchronizationPending = false;
          this._TrySynchronize();
        }

        return;
      }

      // if synchronization has failed, schedule a retry to attempt synchronization again at a future point in time
      this._LogWarn("Synchronization has failed - Scheduling retry");
      this._ScheduleSynchronizationRetry();
    }

    private void _LogInfo(string message) {
      if (!this.synchronizationDebugging) return;

      Debug.Log($"[{this.gameObject.name}] {message}", this);
    }

    private void _LogWarn(string message) {
      if (!this.synchronizationDebugging) return;

      Debug.LogWarning($"[{this.gameObject.name}] {message}", this);
    }

    private void _LogError(string message) {
      if (!this.synchronizationDebugging) return;

      Debug.LogError($"[{this.gameObject.name}] {message}", this);
    }
  }
}
