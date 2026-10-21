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
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;
using VRCTools.World.LocalValues;
using VRCTools.World.Values;

namespace VRCTools.World.PersistedValues {
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Persisted Values/Persisted Int")]
  public class PersistedInt : LocalInt {
    public new const int EVENT_STATE_UPDATED = ValueEvents.STANDARD_STATE_UPDATED;
    public new const int EVENT_STATE_RESET = ValueEvents.STANDARD_STATE_RESET;
    public const int EVENT_LOADED = ValueEvents.PERSISTED_LOADED;
    public const int EVENT_PERSIST = ValueEvents.PERSISTED_PERSIST;
    public new const int EVENT_COUNT = ValueEvents.PERSISTED_COUNT;

    /// <summary>
    ///   Identifies the key with which this value will be persisted.
    /// </summary>
    public string key;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the local state.
    ///   When setting the value, event emission will only take place if the new value differs from the current
    ///   value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public override int State {
      get => this._state;
      set {
        if (!this.Loaded) {
          UdonDebug.Warn(this,
            "Attempted to update value prior to OnPlayerRestored event being invoked - State change has been ignored");
          return;
        }

        if (this._state == value) return;

        this._state = value;
        this._Persist();
        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    /// <summary>
    ///   Identifies whether the value has been populated yet.
    /// </summary>
    public bool Loaded { get; private set; }
    
    [CustomEvent]
    [MustCallBase]
    public override void _Reset() {
      if (!this.Loaded) {
        UdonDebug.Warn(this, "Attempted to reset value prior to OnPlayerRestored event being invoked");
        return;
      }

      base._Reset();
    }

    /// <summary>
    ///   Persists the current value to the local player store.
    /// </summary>
    [CustomEvent]
    public void _Persist() {
      if (!this.Loaded) {
        UdonDebug.Warn(this, "Attempted to persist value prior to OnPlayerRestored event being invoked");
        return;
      }

      if (string.IsNullOrEmpty(this.key)) {
        UdonDebug.Error(this, "Attempted to persist value without key");
        return;
      }

      PlayerData.SetInt(this.key, this.State);
      this._EmitEvent(EVENT_PERSIST);
    }

    [MustCallBase]
    public override void OnPlayerRestored(VRCPlayerApi player) {
      if (!player.isLocal) {
        return;
      }

      this._state = PlayerData.TryGetInt(Networking.LocalPlayer, this.key, out var stored) ? stored : this.defaultValue;

      this.Loaded = true;
      this._EmitEvent(EVENT_LOADED);
      this._EmitEvent(EVENT_STATE_UPDATED);
    }
  }
}
