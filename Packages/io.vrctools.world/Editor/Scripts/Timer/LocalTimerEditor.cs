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

using System;
using UnityEditor;
using VRCTools.Common.Attributes;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Timer;

namespace VRCTools.World.Editor.Timer
{
  [CustomEditor(typeof(LocalTimer))]
  public class LocalTimerEditor : AbstractEventEmitterEditor
  {
    protected override string HelpText => "Pausable timer which counts down for a given duration and emits events" +
                                          "when completed.";

    private SerializedProperty _activateOnStart;
    private SerializedProperty _recurring;
    private SerializedProperty _defaultDuration;
    private SerializedProperty _targetValue;

    [MustCallBase]
    protected override void OnEnable()
    {
      base.OnEnable();

      this._activateOnStart = this.serializedObject.FindProperty(nameof(LocalTimer.activateOnStart));
      this._recurring = this.serializedObject.FindProperty(nameof(LocalTimer.recurring));
      this._defaultDuration = this.serializedObject.FindProperty(nameof(LocalTimer.defaultDuration));
      this._targetValue = this.serializedObject.FindProperty(nameof(LocalTimer.targetValue));
    }

    protected override void RenderInspectorGUI()
    {
      EditorGUILayout.LabelField("Basics", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._activateOnStart);
      EditorGUILayout.PropertyField(this._recurring);
      EditorGUILayout.PropertyField(this._defaultDuration);
      if (this._defaultDuration.floatValue < 1)
      {
        EditorGUILayout.HelpBox(
          "Short intervals (less than one second) may be problematic as the timer may not be able to keep up depending on the frame rate - Depending on your use case, a custom script may be more suitable",
          MessageType.Warning);
      }

      EditorGUILayout.PropertyField(this._targetValue);

      EditorGUILayout.Space(20);
      base.RenderInspectorGUI();
    }
  }
}
