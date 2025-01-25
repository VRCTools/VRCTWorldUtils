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
  ///   Encapsulates a local string value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Local String")]
  public class LocalString : AbstractEventEmitter {
    public const int EVENT_STATE_UPDATED = 0;
    public const int EVENT_COUNT = 1;

    /// <summary>
    ///   Identifies the value with which this local string will be initialized.
    /// </summary>
    public string defaultValue;

    /// <summary>
    ///   Stores the actual local state.
    /// </summary>
    private string _state;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the local state.
    ///   When setting the value, event emission will only take place if the new value differs from the current
    ///   value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public string State {
      get => this._state;
      set {
        if (this._state == value) return;
        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    private void Start() {
      this._state = this.defaultValue;

      this._EmitEvent(EVENT_STATE_UPDATED);
    }
  }
}
