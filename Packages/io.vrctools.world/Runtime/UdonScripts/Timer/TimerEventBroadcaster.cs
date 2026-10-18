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
using VRCTools.World.Utils;

namespace VRCTools.World.Timer
{
    /// <summary>
    /// Utility script which permits broadcasting timer events to standard behaviours.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [AddComponentMenu("Timer/Timer Event Broadcaster")]
    public class TimerEventBroadcaster : UdonSharpBehaviour
    {
        public LocalTimer localTimer;
        
        public UdonSharpBehaviour[] startBehaviours;
        public string[] startEventNames;
        
        public UdonSharpBehaviour[] pauseBehaviours;
        public string[] pauseEventNames;
        
        public UdonSharpBehaviour[] resumeBehaviours;
        public string[] resumeEventNames;
        
        public UdonSharpBehaviour[] finishBehaviours;
        public string[] finishEventNames;
        
        public UdonSharpBehaviour[] stopBehaviours;
        public string[] stopEventNames;
        
        public UdonSharpBehaviour[] resetBehaviours;
        public string[] resetEventNames;

        public DelayMode delayMode;
        public EventTiming delayTiming = EventTiming.Update;
        [Min(0)] public int delayFrames;
        [Min(0)] public float delaySeconds;

        private void Start()
        {
            this.startBehaviours = MappingUtility.CheckCoherentMapping(this.startBehaviours, this.startEventNames, this);
            this.pauseBehaviours = MappingUtility.CheckCoherentMapping(this.pauseBehaviours, this.pauseEventNames, this);
            this.resumeBehaviours = MappingUtility.CheckCoherentMapping(this.resumeBehaviours, this.resumeEventNames, this);
            this.finishBehaviours = MappingUtility.CheckCoherentMapping(this.finishBehaviours, this.finishEventNames, this);
            this.stopBehaviours = MappingUtility.CheckCoherentMapping(this.stopBehaviours, this.stopEventNames, this);
            this.resetBehaviours = MappingUtility.CheckCoherentMapping(this.resetBehaviours, this.resetEventNames, this);

            if (!Utilities.IsValid(this.localTimer))
            {
                Debug.LogError("[Timer Event Broadcaster] Timer is invalid - Disabled");
                this.enabled = false;
                return;
            }
            
            this.localTimer._RegisterHandler(LocalTimer.EVENT_START, this, nameof(this._OnTimerStarted));
            this.localTimer._RegisterHandler(LocalTimer.EVENT_PAUSE, this, nameof(this._OnTimerPaused));
            this.localTimer._RegisterHandler(LocalTimer.EVENT_RESUME, this, nameof(this._OnTimerResumed));
            this.localTimer._RegisterHandler(LocalTimer.EVENT_FINISH, this, nameof(this._OnTimerFinished));
            this.localTimer._RegisterHandler(LocalTimer.EVENT_STOP, this, nameof(this._OnTimerStopped));
            this.localTimer._RegisterHandler(LocalTimer.EVENT_RESET, this, nameof(this._OnTimerReset));
        }

        private void _InvokeEvent(UdonSharpBehaviour behaviour, string eventName)
        {
            switch (this.delayMode)
            {
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
                    Debug.LogError("[Timer Event Broadcaster] Delay mode not implemented: " + this.delayMode, this);
                    break;
            }
        }

        private void _InvokeEvents(string triggerName, UdonSharpBehaviour[] behaviours, string[] eventNames)
        {
            for (var i = 0; i < behaviours.Length; ++i)
            {
                var behaviour = behaviours[i];
                var eventName = eventNames[i];

                if (!Utilities.IsValid(behaviour))
                {
                    Debug.LogError($"[Timer Event Broadcaster] Behaviour {triggerName}#{i} is invalid - Skipped", this);
                    continue;
                }

                if (string.IsNullOrEmpty(eventName))
                {
                    Debug.LogError($"[Timer Event Broadcaster] Event Name for behaviour {triggerName}#{i} has not been set - Skipped", this);
                    continue;
                }
                
                this._InvokeEvent(behaviour, eventName);
            }
        }

        public void _OnTimerStarted()
        {
            this._InvokeEvents("TIMER_STARTED", this.startBehaviours, this.startEventNames);
        }

        public void _OnTimerPaused()
        {
            this._InvokeEvents("TIMER_PAUSED", this.pauseBehaviours, this.pauseEventNames);
        }

        public void _OnTimerResumed()
        {
            this._InvokeEvents("TIMER_RESUMED", this.resumeBehaviours, this.resumeEventNames);
        }

        public void _OnTimerFinished()
        {
            this._InvokeEvents("TIMER_FINISHED", this.finishBehaviours, this.finishEventNames);
        }

        public void _OnTimerStopped()
        {
            this._InvokeEvents("TIMER_STOPPED", this.stopBehaviours, this.stopEventNames);
        }

        public void _OnTimerReset()
        {
            this._InvokeEvents("TIMER_RESET", this.resetBehaviours, this.resetEventNames);
        }
    }
}