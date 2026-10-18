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
using UnityEditor;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Timer;
using VRCTools.World.Utils;

namespace VRCTools.World.Editor.Timer
{
    [CustomEditor(typeof(TimerEventBroadcaster))]
    public class TimerEventBroadcasterEditor : AbstractCustomUdonEditor
    {
        private SerializedProperty _localTimer;

        private SerializedProperty _startBehaviours;
        private SerializedProperty _startEventNames;

        private SerializedProperty _pauseBehaviours;
        private SerializedProperty _pauseEventNames;
        
        private SerializedProperty _resumeBehaviours;
        private SerializedProperty _resumeEventNames;

        private SerializedProperty _finishBehaviours;
        private SerializedProperty _finishEventNames;
        
        private SerializedProperty _stopBehaviours;
        private SerializedProperty _stopEventNames;
        
        private SerializedProperty _resetBehaviours;
        private SerializedProperty _resetEventNames;

        private SerializedProperty _delayMode;
        private SerializedProperty _delayTiming;
        private SerializedProperty _delayFrames;
        private SerializedProperty _delaySeconds;

        private MultiPropertyList _startEvents;
        private MultiPropertyList _pauseEvents;
        private MultiPropertyList _resumeEvents;
        private MultiPropertyList _finishEvents;
        private MultiPropertyList _stopEvents;
        private MultiPropertyList _resetEvents;

        private void OnEnable()
        {
            this._localTimer = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.localTimer));
            
            this._startBehaviours = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.startBehaviours));
            this._startEventNames = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.startEventNames));
            
            this._pauseBehaviours = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.pauseBehaviours));
            this._pauseEventNames = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.pauseEventNames));
            
            this._resumeBehaviours = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.resumeBehaviours));
            this._resumeEventNames = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.resumeEventNames));
            
            this._finishBehaviours = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.finishBehaviours));
            this._finishEventNames = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.finishEventNames));
            
            this._stopBehaviours = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.stopBehaviours));
            this._stopEventNames = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.stopEventNames));
            
            this._resetBehaviours = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.resetBehaviours));
            this._resetEventNames = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.resetEventNames));
            
            this._delayMode = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.delayMode));
            this._delayTiming = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.delayTiming));
            this._delayFrames = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.delayFrames));
            this._delaySeconds = this.serializedObject.FindProperty(nameof(TimerEventBroadcaster.delaySeconds));

            this._startEvents = new MultiPropertyList(
                new GUIContent("Timer Start"),
                this.serializedObject,
                "Behaviour",
                this._startBehaviours,
                new[] { "Event Name" },
                new[] { this._startEventNames });

            this._pauseEvents = new MultiPropertyList(
                new GUIContent("Timer Pause"),
                this.serializedObject,
                "Behaviour",
                this._pauseBehaviours,
                new[] { "Event Name" },
                new[] { this._pauseEventNames });
            
            this._resumeEvents = new MultiPropertyList(
                new GUIContent("Timer Resume"),
                this.serializedObject,
                "Behaviour",
                this._resumeBehaviours,
                new[] { "Event Name" },
                new[] { this._resumeEventNames });

            this._finishEvents = new MultiPropertyList(
                new GUIContent("Timer Finish"),
                this.serializedObject,
                "Behaviour",
                this._finishBehaviours,
                new[] { "Event Name" },
                new[] { this._finishEventNames });
            
            this._stopEvents = new MultiPropertyList(
                new GUIContent("Timer Stop"),
                this.serializedObject,
                "Behaviour",
                this._stopBehaviours,
                new[] { "Event Name" },
                new[] { this._stopEventNames });
            
            this._resetEvents = new MultiPropertyList(
                new GUIContent("Timer Reset"),
                this.serializedObject,
                "Behaviour",
                this._resetBehaviours,
                new[] { "Event Name" },
                new[] { this._resetEventNames });
        }

        protected override void RenderInspectorGUI()
        {
            EditorGUILayout.PropertyField(this._localTimer);
            EditorGUILayout.Space(20);
            
            this._startEvents.DoLayout();
            EditorGUILayout.Space(10);
            this._pauseEvents.DoLayout();
            EditorGUILayout.Space(10);
            this._resumeEvents.DoLayout();
            EditorGUILayout.Space(10);
            this._stopEvents.DoLayout();
            EditorGUILayout.Space(10);
            this._finishEvents.DoLayout();
            EditorGUILayout.Space(10);
            this._resetEvents.DoLayout();
            EditorGUILayout.Space(20);
            
            var delayMode = (DelayMode) this._delayMode.enumValueIndex;
            switch (delayMode)
            {
                case DelayMode.NONE:
                    break;
                case DelayMode.FRAMES:
                    EditorGUILayout.PropertyField(this._delayFrames);
                    break;
                case DelayMode.SECONDS:
                    EditorGUILayout.PropertyField(this._delaySeconds);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUILayout.PropertyField(this._delayTiming);
        }
    }
}