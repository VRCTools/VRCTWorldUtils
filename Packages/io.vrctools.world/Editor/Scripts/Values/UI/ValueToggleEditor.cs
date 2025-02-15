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

using UnityEditor;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Values.UI;

namespace VRCTools.World.Editor.Values.UI {
  [CustomEditor(typeof(ValueToggle))]
  public class ValueToggleEditor : AbstractCustomUdonEditor {
    private SerializedProperty _source;
    private SerializedProperty _localValue;
    private SerializedProperty _synchronizedValue;

    private SerializedProperty _invert;

    protected override string HelpText =>
      "Permits the control of a local boolean via a Toggle UI control.\n\n" +
      "This script is expected to be used with a LocalBoolean component and will match its current state as " +
      "well as permit updating the state so long as the UI element is interactable.";

    private void OnEnable() {
      this._source = this.serializedObject.FindProperty(nameof(ValueToggle.source));
      this._localValue = this.serializedObject.FindProperty(nameof(ValueToggle.localValue));
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(ValueToggle.synchronizedValue));

      this._invert = this.serializedObject.FindProperty(nameof(ValueToggle.invert));
    }

    protected override void RenderInspectorGUI() {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Source/Target"), this._source, this._localValue,
        this._synchronizedValue, "Invalid or unspecified local value reference.\n\n" +
                                 "This component will disable itself upon startup.");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._invert);
    }
  }
}
