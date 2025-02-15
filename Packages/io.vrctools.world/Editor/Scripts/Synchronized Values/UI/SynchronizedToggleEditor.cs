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
using VRCTools.World.SynchronizedValues.UI;
using UnityEditor;

namespace VRCTools.World.Editor.SynchronizedValues.UI {
  [CustomEditor(typeof(SynchronizedToggle))]
  public class SynchronizedToggleEditor : AbstractCustomUdonEditor {
    private SerializedProperty _invert;
    private SerializedProperty _synchronizedValue;

    protected override string HelpText =>
      "Permits the control of a synchronized boolean via a Toggle UI control.\n\n" +
      "This script is expected to be used with a SynchronizedBoolean component and will match its current state as " +
      "well as permit updating the state so long as the UI element is interactable.";

    private void OnEnable() {
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(SynchronizedToggle.synchronizedValue));
      this._invert = this.serializedObject.FindProperty(nameof(SynchronizedToggle.invert));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._synchronizedValue);
      if (this._synchronizedValue.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified synchronized value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._invert);
    }
  }
}
