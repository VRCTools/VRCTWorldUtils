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

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon.Common.Enums;
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Applicators/Boolean Event Applicator")]
  public class BooleanEventApplicator : UdonSharpBehaviour {
    public ValueType source;
    public LocalBoolean localValue;
    public SynchronizedBoolean synchronizedValue;

    public UdonSharpBehaviour[] falseBehaviours;
    public string[] falseEventNames;

    public UdonSharpBehaviour[] trueBehaviours;
    public string[] trueEventNames;

    public DelayMode delayMode;
    public EventTiming delayTiming = EventTiming.Update;
    [Min(0)]
    public int delayFrames;
    [Min(0)]
    public float delaySeconds;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localValue, this.synchronizedValue))
      {
        UdonDebug.Error(this, "Source value is invalid - Disabled");
        this.enabled = false;
        return;
      }
      
      this.falseBehaviours = MappingUtility.CheckCoherentMapping(this.falseBehaviours, this.falseEventNames, this);
      this.trueBehaviours = MappingUtility.CheckCoherentMapping(this.trueBehaviours, this.trueEventNames, this);

      ValueUtility.RegisterUpdateHandler(this.source, this.localValue, this.synchronizedValue, this,
        nameof(this._OnStateUpdated));
    }

    [CustomEvent]
    public void _OnStateUpdated() {
      var state = ValueUtility.GetValue(this.source, this.localValue, this.synchronizedValue);

      var behaviourSource = state ? this.trueBehaviours : this.falseBehaviours;
      var eventNameSource = state ? this.trueEventNames : this.falseEventNames;

      for (var i = 0; i < behaviourSource.Length; ++i) {
        var behaviour = behaviourSource[i];
        var eventName = eventNameSource[i];

        if (!Utilities.IsValid(behaviour)) {
          UdonDebug.Warn(this, $"Target behavior for state {state} at index #{i} is no longer valid");
          continue;
        }

        if (string.IsNullOrEmpty(eventName)) {
          UdonDebug.Warn(this, $"Target behavior for state {state} at index #{i} has no event name specified");
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
            UdonDebug.Error(this, "Delay mode not implemented");
            break;
        }
      }
    }
  }
}
