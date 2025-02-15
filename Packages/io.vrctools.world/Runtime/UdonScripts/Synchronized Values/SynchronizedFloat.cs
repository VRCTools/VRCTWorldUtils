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
using VRCTools.World.Abstractions;

namespace VRCTools.World.SynchronizedValues {
  /// <summary>
  ///   Encapsulates a synchronized float value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Synchronized Values/Synchronized Float")]
  public class SynchronizedFloat : AbstractSynchronizedBehaviour {
    public const int EVENT_STATE_UPDATED = 0;
    public const int EVENT_COUNT = 1;

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
    public float _previousState;

    /// <summary>
    ///   Stores the actual synchronized state.
    /// </summary>
    [UdonSynced]
    private float state;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the synchronized state.
    ///   When setting the value, synchronization and event emission will only take place if the new value differs from
    ///   the current value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public float State {
      get => this.state;
      set {
        if (this.preventNuisanceUpdates && Mathf.Abs(value - this.state) < this.minimalUpdateDifference) return;

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

      if (this.minimalUpdateDifference < Mathf.Epsilon) {
        Debug.LogError(
          $"Minimum update difference of {this.minimalUpdateDifference} exceeds smallest permitted value of {Mathf.Epsilon} - Value will be clamped",
          this);
        this.minimalUpdateDifference = Mathf.Epsilon;
      }

      base.Start();

      this._EmitEvent(EVENT_STATE_UPDATED);
    }

    public void _IncrementBy(float delta) { this.State += delta; }

    public void _DecrementBy(float delta) { this.State -= delta; }

    public void _MultiplyBy(float factor) { this.State *= factor; }

    public void _DivideBy(float divisor) { this.State /= divisor; }

    public override void OnDeserialization() {
      base.OnDeserialization();

      var updated = !this.preventNuisanceUpdates ||
                    Mathf.Abs(this._previousState - this.state) >= this.minimalUpdateDifference;
      this._previousState = this.state;

      if (updated) this._EmitEvent(EVENT_STATE_UPDATED);
    }
  }
}
