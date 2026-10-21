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
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;

namespace VRCTools.World.Timer
{
    /// <summary>
    /// Displays the current state of a connected <see cref="localTimer"/> within UI elements.
    /// </summary>
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [AddComponentMenu("Timer/Timer Label")]
    public class TimerLabel : UdonSharpBehaviour
    {
        public LocalTimer localTimer;

        public TextMeshProUGUI remainingTimeLabel;
        public TextMeshProUGUI stateStoppedLabel;
        public TextMeshProUGUI statePausedLabel;
        public TextMeshProUGUI stateRunningLabel;

        [Min(0)]
        public float remainingTimeUpdateFrequency = 1f;
        public string remainingTimeFormat = @"hh\:mm\:ss";
        public bool setActiveRemainingLabel = true;

        private string _remainingTimeTemplate;

        private bool _remainingTimeLabelPresent;
        private bool _stateStoppedLabelPresent;
        private bool _statePausedLabelPresent;
        private bool _stateRunningLabelPresent;

        private float _remainingTimeUpdateTimer = -1;
        
        private void Start()
        {
            if (!Utilities.IsValid(this.localTimer))
            {
                UdonDebug.Error(this, "Invalid timer value given - Disabled");
                this.enabled = false;
                return;
            }
            
            this._remainingTimeLabelPresent = Utilities.IsValid(this.remainingTimeLabel);
            this._stateStoppedLabelPresent = Utilities.IsValid(this.stateStoppedLabel);
            this._statePausedLabelPresent = Utilities.IsValid(this.statePausedLabel);
            this._stateRunningLabelPresent = Utilities.IsValid(this.stateRunningLabel);

            if (!this._remainingTimeLabelPresent && !this._stateStoppedLabelPresent && !this._statePausedLabelPresent &&
                !this._stateRunningLabelPresent)
            {
                UdonDebug.Error(this, "No labels given (there is nothing to do) - Disabled");
                this.enabled = false;
            }

            if (this._remainingTimeLabelPresent)
            {
                this._remainingTimeTemplate = this.remainingTimeLabel.text;

                if (string.IsNullOrEmpty(this._remainingTimeTemplate))
                {
                    this._remainingTimeTemplate = "{0}";
                }
            }
            
            this.localTimer._RegisterHandler(LocalTimer.EVENT_START, this, nameof(this._OnTimerStateUpdated));
            this.localTimer._RegisterHandler(LocalTimer.EVENT_PAUSE, this, nameof(this._OnTimerStateUpdated));
            this.localTimer._RegisterHandler(LocalTimer.EVENT_RESUME, this, nameof(this._OnTimerStateUpdated));
            this.localTimer._RegisterHandler(LocalTimer.EVENT_STOP, this, nameof(this._OnTimerStateUpdated));

            this._UpdateState();
        }

        private void Update()
        {
            if (!this._remainingTimeLabelPresent || this.remainingTimeUpdateFrequency < Mathf.Epsilon)
            {
                return;
            }
            
            this._remainingTimeUpdateTimer -= Time.deltaTime;
            if (this._remainingTimeUpdateTimer > 0)
            {
                return;
            }
            
            this._remainingTimeUpdateTimer = this.remainingTimeUpdateFrequency;
            this._UpdateRemainingTime();
        }

        private void OnDestroy()
        {
            if (!Utilities.IsValid(this.localTimer))
            {
                return;
            }
            
            this.localTimer._UnregisterHandler(this);
        }

        [CustomEvent]
        public void _OnTimerStateUpdated()
        {
            this._UpdateState();
        }

        [CustomEvent]
        public void _UpdateState()
        {
            var state = this.localTimer.State;
            
            if (this._remainingTimeLabelPresent)
            {
                this._UpdateRemainingTime();

                if (this.setActiveRemainingLabel)
                {
                    this.remainingTimeLabel.gameObject.SetActive(state == TimerState.Running);
                }
            }

            if (this._stateStoppedLabelPresent)
            {
                this.stateStoppedLabel.gameObject.SetActive(state == TimerState.Stopped);
            }

            if (this._statePausedLabelPresent)
            {
                this.statePausedLabel.gameObject.SetActive(state == TimerState.Paused);
            }

            if (this._stateRunningLabelPresent)
            {
                this.stateRunningLabel.gameObject.SetActive(state == TimerState.Running);
            }
        }
    
        [CustomEvent]
        public void _UpdateRemainingTime()
        {

            if (!this._remainingTimeLabelPresent)
            {
                return;
            }
            
            var remainingDuration = this.localTimer.RemainingTime;
            this.remainingTimeLabel.text = string.Format(this._remainingTimeTemplate,
                    TimeSpan.FromSeconds(remainingDuration).ToString(this.remainingTimeFormat));
        }
    }
}