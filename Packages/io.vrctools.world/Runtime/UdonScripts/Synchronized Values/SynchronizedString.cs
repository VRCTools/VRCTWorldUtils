using VRCTools.World.Abstractions;
using UdonSharp;
using UnityEngine;

namespace VRCTools.World.SynchronizedValues {
  /// <summary>
  ///   Encapsulates a synchronized string value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Synchronized Values/Synchronized String")]
  public class SynchronizedString : AbstractSynchronizedBehaviour {
    public const int EVENT_STATE_UPDATED = 0;
    public const int EVENT_COUNT = 1;

    /// <summary>
    ///   Identifies the value with which this synchronized boolean will be initialized.
    /// </summary>
    public string defaultValue;

    /// <summary>
    ///   Stores the previously known state.
    /// </summary>
    private string _previousState;

    /// <summary>
    ///   Stores the actual synchronized state.
    /// </summary>
    [UdonSynced]
    private string state;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the synchronized state.
    ///   When setting the value, synchronization and event emission will only take place if the new value differs from
    ///   the current value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public string State {
      get => this.state;
      set {
        if (this.state == value) return;

        this._EnsureOwnership();

        this.state = this._previousState = value;

        this._TrySynchronize();
        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    protected override void Start() {
      this.state = this._previousState = this.defaultValue;

      base.Start();

      this._EmitEvent(EVENT_STATE_UPDATED);
    }

    public override void OnDeserialization() {
      base.OnDeserialization();

      var updated = this._previousState != this.state;
      this._previousState = this.state;

      if (updated) this._EmitEvent(EVENT_STATE_UPDATED);
    }
  }
}
