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
  [CustomEditor(typeof(LocalInputField))]
  public class LocalInputFieldEditor : AbstractCustomUdonEditor {
    private SerializedProperty _localValue;

    protected override string HelpText =>
      "Permits the control of a local string value via an InputField.\n\n" +
      "This script is expected to be combined with a LocalString component and will match its current state.";

    private void OnEnable() {
      this._localValue = this.serializedObject.FindProperty(nameof(LocalInputField.localValue));
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
