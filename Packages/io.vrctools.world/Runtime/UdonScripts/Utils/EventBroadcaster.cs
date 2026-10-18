// Copyright 2026 .start <https://dotstart.tv>
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

using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Enums;

namespace VRCTools.World.Utils {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Utilities/Event Broadcaster")]
  public class EventBroadcaster : UdonSharpBehaviour {
    public UdonSharpBehaviour[] behaviours;
    public string[] eventNames;

    public DelayMode delayMode;
    public EventTiming delayTiming = EventTiming.Update;
    [Min(0)]
    public int delayFrames;
    [Min(0)]
    public float delaySeconds;

    private void Start() {
      this.behaviours = MappingUtility.CheckCoherentMapping(this.behaviours, this.eventNames, this);
    }

    public void _Trigger() {
      for (var i = 0; i < this.behaviours.Length; ++i) {
        var behaviour = this.behaviours[i];
        var eventName = this.eventNames[i];

        if (!Utilities.IsValid(behaviour)) {
          Debug.LogError($"[Event Broadcaster] Behaviour at index #{i} is invalid - Skipped", this);
          continue;
        }

        if (string.IsNullOrEmpty(eventName)) {
          Debug.LogError($"[Event Broadcaster] Event name for behaviour at index #{i} has not been set - Skipped",
            this);
          continue;
        }

        switch (this.delayMode) {
          case DelayMode.NONE:
            behaviour.SendCustomEvent(eventName);
            break;
          case DelayMode.FRAMES:
            behaviour.SendCustomEventDelayedFrames(eventName, this.delayFrames, this.delayTiming);
            break;
          case DelayMode.SECONDS:
            behaviour.SendCustomEventDelayedSeconds(eventName, this.delaySeconds, this.delayTiming);
            break;
          default:
            Debug.LogError("[Event Broadcaster] Delay mode not implemented", this);
            break;
        }
      }
    }
  }
}
