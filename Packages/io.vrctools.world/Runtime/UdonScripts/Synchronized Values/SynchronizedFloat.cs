// Copyright 2025 .start <https://dotstart.tv>
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;
using VRCTools.Event;
using VRCTools.World.Values;

namespace VRCTools.World.SynchronizedValues
{
  /// <summary>
  ///   Encapsulates a synchronized float value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Synchronized Values/Synchronized Float")]
  public class SynchronizedFloat : AbstractSynchronizedEventEmitter
  {
    public const int EVENT_STATE_UPDATED = ValueEvents.STANDARD_STATE_UPDATED;
    public const int EVENT_STATE_RESET = ValueEvents.STANDARD_STATE_RESET;
    public const int EVENT_COUNT = ValueEvents.STANDARD_COUNT;

    /// <summary>
    ///   Identifies the value with which this synchronized float will be initialized.
    /// </summary>
    public float defaultValue;

    /// <summary>
    ///   Specifies whether the value shall be capped between a pre-determined minimum and maximum value.
    /// </summary>
    public bool restrictValue;

    /// <summary>
    ///   Defines the minimum value which the state may be set to when <see cref="restrictValue" /> is enabled.
    /// </summary>
    public float restrictedMinimumValue;

    /// <summary>
    ///   Defines the maximum value which the state may be set to when <see cref="restrictValue" /> is enabled.
    /// </summary>
    public float restrictedMaximumValue = 1;

    /// <summary>
    ///   Specifies whether this synchronized value shall prevent updates and synchronizations if the value is adjusted by
    ///   an incredibly small irrelevant margin.
    /// </summary>
    public bool preventNuisanceUpdates;

    /// <summary>
    ///   Defines the margin by which the value needs to change in order to trigger synchronization and event emission
    ///   when <see cref="preventNuisanceUpdates" /> is enabled.
    /// </summary>
    public float minimalUpdateDifference = Mathf.Epsilon;

    /// <summary>
    ///   Stores the previously known state.
    /// </summary>
    private float _previousState;

    /// <summary>
    ///   Stores the actual synchronized state.
    /// </summary>
    [UdonSynced] private float state;

    /// <summary>
    ///   Stores a counter which tracks the number of resets performed on this value.
    /// </summary>
    [UdonSynced] private int resetCounter;

    public override int EventCount => EVENT_COUNT;

    private int _previousResetCounterValue;

    /// <summary>
    ///   Retrieves or updates the synchronized state.
    ///   When setting the value, synchronization and event emission will only take place if the new value differs from
    ///   the current value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public float State
    {
      get => this.state;
      set
      {
        var newValue = this.restrictValue
          ? Mathf.Clamp(value, this.restrictedMinimumValue, this.restrictedMaximumValue)
          : value;

        if (!this.IsLocallyOwned() && !this.IsStateInitialized)
        {
          UdonDebug.Warn(this, "Attempted state update prior to initialization - Ignored");
          return;
        }

        if (Mathf.Abs(newValue - this.state) <
            (this.preventNuisanceUpdates ? this.minimalUpdateDifference : Mathf.Epsilon))
        {
          return;
        }

        if (float.IsNaN(this.state) && float.IsNaN(newValue))
        {
          return;
        }

        if (float.IsInfinity(this.state) && float.IsInfinity(newValue))
        {
          return;
        }

        this._EnsureOwnership();
        this.state = this._previousState = newValue;
        this._TrySynchronize();
        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    [MustCallBase]
    protected override void Start()
    {
      if (this.restrictedMinimumValue > this.restrictedMaximumValue)
      {
        UdonDebug.Error(this,
          $"Minimum value of {this.restrictedMinimumValue} exceeds maximum value of {this.restrictedMaximumValue} - Restriction will be ignored");
        this.restrictValue = false;
      }

      this.state = this._previousState = (this.restrictValue
        ? Mathf.Clamp(this.defaultValue, this.restrictedMinimumValue, this.restrictedMaximumValue)
        : this.defaultValue);

      if (this.minimalUpdateDifference < Mathf.Epsilon)
      {
        UdonDebug.Error(this,
          $"Minimum update difference of {this.minimalUpdateDifference} falls short of smallest permitted value of {Mathf.Epsilon} - Value will be clamped");
        this.minimalUpdateDifference = Mathf.Epsilon;
      }

      base.Start();
    }

    protected override void _OnStateInitialized()
    {
      this._EmitEvent(EVENT_STATE_UPDATED);
    }

    /// <summary>
    ///   Reverts the current value back to the default value.
    /// </summary>
    [CustomEvent]
    [MustCallBase]
    public virtual void _Reset()
    {
      if (!this.IsLocallyOwned() && !this.IsStateInitialized)
      {
        UdonDebug.Warn(this, "Attempted state reset prior to initialization - Ignored");
        return;
      }

      var defaultValue = this.restrictValue
        ? Mathf.Clamp(this.defaultValue, this.restrictedMinimumValue, this.restrictedMaximumValue)
        : this.defaultValue;

      // ignore if the current state already reflects defaults as we do not wish to notify anyone in this case
      var currentlyDefault = Mathf.Abs(this.state - defaultValue) <
                             (this.preventNuisanceUpdates ? this.minimalUpdateDifference : Mathf.Epsilon);
      if (currentlyDefault)
      {
        return;
      }

      this._EnsureOwnership();

      this._previousResetCounterValue = ++this.resetCounter;
      this.State = defaultValue;

      this._EmitEvent(EVENT_STATE_RESET);
    }

    [CustomEvent]
    public void _Zero()
    {
      this.State = 0;
    }

    [MustCallBase]
    public override void OnDeserialization()
    {
      var updated = Mathf.Abs(this.state - this._previousState) >=
        (this.preventNuisanceUpdates ? this.minimalUpdateDifference : Mathf.Epsilon) && this.IsStateInitialized;
      if (!this.IsStateInitialized || updated)
      {
        this._previousState = this.state;
      }

      if (updated)
      {
        this._EmitEvent(EVENT_STATE_UPDATED);
      }

      var reset = this._previousResetCounterValue != this.resetCounter && this.IsStateInitialized;
      this._previousResetCounterValue = this.resetCounter;

      if (reset) this._EmitEvent(EVENT_STATE_RESET);

      // base has to be invoked last here as it also takes care of flagging the object as initialized thus requiring
      // the value logic to run first
      base.OnDeserialization();
    }
  }
}
