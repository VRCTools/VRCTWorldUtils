using VRCTools.World.Abstractions;
using UdonSharp;
using UnityEngine;

namespace VRCTools.World.SynchronizedValues {
  /// <summary>
  ///   Encapsulates a synchronized color value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Synchronized Values/Synchronized Color")]
  public class SynchronizedColor : AbstractSynchronizedBehaviour {
    public const int EVENT_STATE_UPDATED = 0;
    public const int EVENT_COUNT = 1;

    /// <summary>
    ///   Identifies the value with which this synchronized color will be initialized.
    /// </summary>
    public Color defaultValue;

    /// <summary>
    ///   Stores the previously known state.
    /// </summary>
    private Color _previousState;

    /// <summary>
    ///   Stores the actual synchronized state.
    /// </summary>
    [UdonSynced]
    private Color state;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the synchronized state.
    ///   When setting the value, synchronization and event emission will only take place if the new value differs from
    ///   the current value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public Color State {
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

      var updated = this.state != this._previousState;
      this._previousState = this.state;

      if (updated) this._EmitEvent(EVENT_STATE_UPDATED);
    }
  }
}
