﻿// Copyright 2025 .start <https://dotstart.tv>
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

using UnityEditor;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.LocalValues.UI;
using VRCTools.World.Values.UI;

namespace VRCTools.World.Editor.Values.UI {
  [CustomEditor(typeof(ValueToggleButton))]
  public class ValueToggleButtonEditor : AbstractCustomUdonEditor {
    private SerializedProperty _source;
    private SerializedProperty _localValue;
    private SerializedProperty _synchronizedValue;

    private SerializedProperty _activeColor;
    private SerializedProperty _inactiveColor;
    private SerializedProperty _alternateLabel;
    private SerializedProperty _onLabel;
    private SerializedProperty _offLabel;

    private SerializedProperty _invert;

    protected override string HelpText =>
      "Permits the control of a local boolean value through a button control which toggles its state back and " +
      "forth.\n\n" +
      "This script is meant to be used with a LocalBoolean component and will visually update the attached " +
      "Button component if configured to do so.";

    private void OnEnable() {
      this._source = this.serializedObject.FindProperty(nameof(ValueToggleButton.source));
      this._localValue = this.serializedObject.FindProperty(nameof(ValueToggleButton.localValue));
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(ValueToggleButton.synchronizedValue));

      this._activeColor = this.serializedObject.FindProperty(nameof(ValueToggleButton.activeColor));
      this._inactiveColor = this.serializedObject.FindProperty(nameof(ValueToggleButton.inactiveColor));
      this._alternateLabel = this.serializedObject.FindProperty(nameof(ValueToggleButton.alternateLabel));
      this._onLabel = this.serializedObject.FindProperty(nameof(ValueToggleButton.onLabel));
      this._offLabel = this.serializedObject.FindProperty(nameof(ValueToggleButton.offLabel));

      this._invert = this.serializedObject.FindProperty(nameof(ValueToggleButton.invert));
    }

    protected override void RenderInspectorGUI() {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Source/Target"), this._source, this._localValue,
        this._synchronizedValue, "Invalid or unspecified local value reference.\n\n" +
                                 "This component will disable itself upon startup.");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._activeColor);
      EditorGUILayout.PropertyField(this._inactiveColor);
      EditorGUILayout.Space(5);
      EditorGUILayout.PropertyField(this._alternateLabel);
      EditorGUI.BeginDisabledGroup(!this._alternateLabel.boolValue);
      {
        EditorGUILayout.PropertyField(this._onLabel);
        EditorGUILayout.PropertyField(this._offLabel);
      }
      EditorGUI.EndDisabledGroup();
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._invert);
    }
  }
}
