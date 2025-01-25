using VRCTools.World.Abstractions;
using UdonSharp;
using UnityEngine;

namespace VRCTools.World.SynchronizedValues {
  /// <summary>
  ///   Encapsulates a synchronized int value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Synchronized Values/Synchronized Int")]
  public class SynchronizedInt : AbstractSynchronizedBehaviour {
    public const int EVENT_STATE_UPDATED = 0;
    public const int EVENT_COUNT = 1;

    /// <summary>
    ///   Identifies the value with which this synchronized int will be initialized.
    /// </summary>
    public int defaultValue;

    /// <summary>
    ///   Specifies whether the value shall be capped between a pre-determined minimum and maximum value.
    /// </summary>
    public bool restrictValue;

    /// <summary>
    ///   Defines the minimum value which the state may be set to when <see cref="restrictValue" /> is enabled.
    /// </summary>
    public int restrictedMinimumValue;

    /// <summary>
    ///   Defines the maximum value which the state may be set to when <see cref="restrictValue" /> is enabled.
    /// </summary>
    public int restrictedMaximumValue = 1;

    /// <summary>
    ///   Stores the previously known state.
    /// </summary>
    private int _previousState;

    /// <summary>
    ///   Stores the actual synchronized state.
    /// </summary>
    [UdonSynced]
    private int state;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the synchronized state.
    ///   When setting the value, synchronization and event emission will only take place if the new value differs from
    ///   the current value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public int State {
      get => this.state;
      set {
        if (this.state == value) return;

        this._EnsureOwnership();

        if (this.restrictValue)
          this.state = this._previousState
            = Mathf.Max(this.restrictedMinimumValue, Mathf.Min(this.restrictedMaximumValue, value));
        else
          this.state = this._previousState = value;

        this._TrySynchronize();
        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    protected override void Start() {
      this.state = this._previousState = this.defaultValue;

      if (this.restrictedMinimumValue > this.restrictedMaximumValue) {
        Debug.LogError(
          $"Minimum value of {this.restrictedMinimumValue} exceeds maximum value of {this.restrictedMaximumValue} - Restriction will be ignored",
          this);
        this.restrictValue = false;
      }

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
