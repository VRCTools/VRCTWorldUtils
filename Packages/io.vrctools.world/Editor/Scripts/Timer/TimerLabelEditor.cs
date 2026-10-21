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
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Timer;

namespace VRCTools.World.Editor.Timer
{
  [CustomEditor(typeof(TimerLabel))]
  public class TimerLabelEditor : AbstractCustomUdonEditor
  {
    protected override string HelpText => "Displays the current state of a timer within a Unity UI.";

    private SerializedProperty _localTimer;

    private SerializedProperty _remainingTimeLabel;
    private SerializedProperty _stateStoppedLabel;
    private SerializedProperty _statePausedLabel;
    private SerializedProperty _stateRunningLabel;

    private SerializedProperty _remainingTimeUpdateFrequency;
    private SerializedProperty _remainingTimeFormat;
    private SerializedProperty _setActiveRemainingLabel;

    private void OnEnable()
    {
      this._localTimer = this.serializedObject.FindProperty(nameof(TimerLabel.localTimer));

      this._remainingTimeLabel = this.serializedObject.FindProperty(nameof(TimerLabel.remainingTimeLabel));
      this._stateStoppedLabel = this.serializedObject.FindProperty(nameof(TimerLabel.stateStoppedLabel));
      this._statePausedLabel = this.serializedObject.FindProperty(nameof(TimerLabel.statePausedLabel));
      this._stateRunningLabel = this.serializedObject.FindProperty(nameof(TimerLabel.stateRunningLabel));

      this._remainingTimeUpdateFrequency =
        this.serializedObject.FindProperty(nameof(TimerLabel.remainingTimeUpdateFrequency));
      this._remainingTimeFormat = this.serializedObject.FindProperty(nameof(TimerLabel.remainingTimeFormat));
      this._setActiveRemainingLabel = this.serializedObject.FindProperty(nameof(TimerLabel.setActiveRemainingLabel));
    }

    protected override void RenderInspectorGUI()
    {
      EditorGUILayout.PropertyField(this._localTimer);
      EditorGUILayout.Space(20);

      EditorGUILayout.PropertyField(this._stateStoppedLabel);
      EditorGUILayout.PropertyField(this._statePausedLabel);
      EditorGUILayout.PropertyField(this._stateRunningLabel);

      EditorGUILayout.Space(20);
      EditorGUILayout.PropertyField(this._remainingTimeUpdateFrequency);
      EditorGUILayout.PropertyField(this._remainingTimeFormat);
      EditorGUILayout.PropertyField(this._remainingTimeLabel);
      EditorGUILayout.PropertyField(this._setActiveRemainingLabel);
    }
  }
}
