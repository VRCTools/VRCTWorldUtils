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

using VRCTools.World.Editor.Abstractions;
using UnityEditor;
using VRCTools.World.LocalValues.UI;

namespace VRCTools.World.Editor.LocalValues.UI {
  [CustomEditor(typeof(LocalToggleButton))]
  public class LocalToggleButtonEditor : AbstractCustomUdonEditor {
    private SerializedProperty _activeColor;
    private SerializedProperty _inactiveColor;

    private SerializedProperty _invert;

    private SerializedProperty _localValue;

    protected override string HelpText =>
      "Permits the control of a local boolean value through a button control which toggles its state back and " +
      "forth.\n\n" +
      "This script is meant to be used with a LocalBoolean component and will visually update the attached " +
      "Button component if configured to do so.";

    private void OnEnable() {
      this._localValue = this.serializedObject.FindProperty(nameof(LocalToggleButton.localValue));

      this._activeColor = this.serializedObject.FindProperty(nameof(LocalToggleButton.activeColor));
      this._inactiveColor = this.serializedObject.FindProperty(nameof(LocalToggleButton.inactiveColor));

      this._invert = this.serializedObject.FindProperty(nameof(LocalToggleButton.invert));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._localValue);
      if (this._localValue.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified local value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Appearance", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._activeColor);
      EditorGUILayout.PropertyField(this._inactiveColor);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._invert);
    }
  }
}
