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
using VRCTools.Common.Attributes;
using VRCTools.Event;
using VRCTools.World.Values;

namespace VRCTools.World.LocalValues {
  /// <summary>
  ///   Encapsulates a local 2D texture value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Local Texture2D")]
  public class LocalTexture2D : AbstractEventEmitter {
    public const int EVENT_STATE_UPDATED = ValueEvents.STANDARD_STATE_UPDATED;
    public const int EVENT_STATE_RESET = ValueEvents.STANDARD_STATE_RESET;
    public const int EVENT_COUNT = ValueEvents.STANDARD_COUNT;

    /// <summary>
    ///   Identifies the value with which this local texture will be initialized.
    /// </summary>
    public Texture2D defaultValue;

    /// <summary>
    ///   Stores the actual local state.
    /// </summary>
    private Texture2D _state;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    ///   Retrieves or updates the local state.
    ///   When setting the value, event emission will only take place if the new value differs from the current
    ///   value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public virtual Texture2D State {
      get => this._state;
      set {
        if (this._state == value) return;

        this._state = value;
        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    [MustCallBase]
    protected virtual void Start() {
      this._state = this.defaultValue;

      this._EmitEvent(EVENT_STATE_UPDATED);
    }

    [CustomEvent]
    [MustCallBase]
    public virtual void _Reset() {
      // ignore if the current state already reflects defaults as we do not wish to notify anyone in this case
      if (this._state == this.defaultValue)
      {
        return;
      }
      
      this.State = this.defaultValue;
      this._EmitEvent(EVENT_STATE_RESET);
    }
  }
}
