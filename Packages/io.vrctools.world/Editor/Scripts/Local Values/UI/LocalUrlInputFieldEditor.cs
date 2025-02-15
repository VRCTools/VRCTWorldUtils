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
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.LocalValues.UI;

namespace VRCTools.World.Editor.LocalValues.UI {
  [CustomEditor(typeof(LocalUrlInputField))]
  public class LocalUrlInputFieldEditor : AbstractCustomUdonEditor {
    private SerializedProperty _localValue;

    protected override string HelpText =>
      "Permits the control of a local URL via an InputField control.\n\n" +
      "This script is expected to be used with a LocalUrl component and will match its current state as " +
      "well as permit updating the state so long as the UI element is interactable.";

    private void OnEnable() {
      this._localValue = this.serializedObject.FindProperty(nameof(LocalUrlInputField.localValue));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._localValue);
      if (this._localValue.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified local value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
    }
  }
}
