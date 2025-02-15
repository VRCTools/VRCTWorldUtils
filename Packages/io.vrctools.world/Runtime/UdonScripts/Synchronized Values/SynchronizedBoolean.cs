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
using VRCTools.World.Abstractions;
using UdonSharp;
using UnityEngine;

namespace VRCTools.World.SynchronizedValues {
  /// <summary>
  ///   Encapsulates a synchronized boolean value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Synchronized Values/Synchronized Boolean")]
  public class SynchronizedBoolean : AbstractSynchronizedBehaviour {
    public const int EVENT_STATE_UPDATED = 0;
    public const int EVENT_COUNT = 1;

    /// <summary>
    ///   Identifies the value with which this synchronized boolean will be initialized.
    /// </summary>
    public bool defaultValue;

    /// <summary>
    ///   Stores the previously known state.
    /// </summary>
    private bool _previousState;

    /// <summary>
    ///   Stores the actual synchronized state.
    /// </summary>
    [UdonSynced]
    private bool state;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the synchronized state.
    ///   When setting the value, synchronization and event emission will only take place if the new value differs from
    ///   the current value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public bool State {
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

    /// <summary>
    ///   Toggles the current value of this synchronized value (e.g. when true, toggles to false; when false, toggles to
    ///   true).
    /// </summary>
    public void _Toggle() { this.State = !this.State; }

    /// <summary>
    ///   Sets the synchronized value to true.
    ///   When the synchronized value is already true, no synchronization or event emission takes place.
    /// </summary>
    public void _SetTrue() { this.State = true; }

    /// <summary>
    ///   Sets the synchronized value to false.
    ///   When the synchronized value is already false, no synchronization or event emission takes place.
    /// </summary>
    public void _SetFalse() { this.State = false; }

    public override void OnDeserialization() {
      base.OnDeserialization();

      var updated = this._previousState != this.state;
      this._previousState = this.state;

      if (updated) this._EmitEvent(EVENT_STATE_UPDATED);
    }
  }
}
