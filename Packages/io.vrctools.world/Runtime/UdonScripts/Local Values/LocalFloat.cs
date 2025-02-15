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
using VRCTools.Event;

namespace VRCTools.World.LocalValues {
  /// <summary>
  ///   Encapsulates a local float value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Local Float")]
  public class LocalFloat : AbstractEventEmitter {
    public const int EVENT_STATE_UPDATED = 0;
    public const int EVENT_COUNT = 1;

    /// <summary>
    ///   Identifies the value with which this local float will be initialized.
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
    ///   Stores the actual local state.
    /// </summary>
    private float _state;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the local state.
    ///   When setting the value, event emission will only take place if the new value differs from the current
    ///   value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public float State {
      get => this._state;
      set {
        if (this.preventNuisanceUpdates && Mathf.Abs(value - this._state) < this.minimalUpdateDifference) return;

        this._state = this.restrictValue
          ? Mathf.Max(this.restrictedMinimumValue, Mathf.Min(this.restrictedMaximumValue, value))
          : value;

        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    private void Start() {
      this._state = this.defaultValue;

      if (this.restrictedMinimumValue > this.restrictedMaximumValue) {
        Debug.LogError(
          $"Minimum value of {this.restrictedMinimumValue} exceeds maximum value of {this.restrictedMaximumValue} - Restriction will be ignored",
          this);
        this.restrictValue = false;
      }

      if (this.minimalUpdateDifference < Mathf.Epsilon) {
        Debug.LogError(
          $"Minimum update difference of {this.minimalUpdateDifference} exceeds smallest permitted value of {Mathf.Epsilon} - Value will be clamped",
          this);
        this.minimalUpdateDifference = Mathf.Epsilon;
      }

      this._EmitEvent(EVENT_STATE_UPDATED);
    }
  }
}
