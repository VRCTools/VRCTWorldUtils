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
using VRCTools.Event;
using VRCTools.World.Utils;

namespace VRCTools.World.Player {
  /// <summary>
  /// Invokes a number of handlers when an object is interacted with by a player.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Player/Player Interact Trigger")]
  public class PlayerInteractTrigger : AbstractEventEmitter {
    public const int EVENT_INTERACT = 0;
    public const int EVENT_COUNT = 1;

    public UdonSharpBehaviour[] behaviours;
    public string[] eventNames;

    public override int EventCount => EVENT_COUNT;

    private void Start() {
      this.behaviours = MappingUtility.CheckCoherentMapping(this.behaviours, this.eventNames, this);
    }

    public override void Interact() {
      for (var i = 0; i < this.behaviours.Length; i++) {
        var behaviour = this.behaviours[i];
        var eventName = this.eventNames[i];
        if (!Utilities.IsValid(behaviour) || string.IsNullOrEmpty(eventName)) {
          continue;
        }

        behaviour.SendCustomEvent(eventName);
      }

      this._EmitEvent(EVENT_INTERACT);
    }
  }
}
