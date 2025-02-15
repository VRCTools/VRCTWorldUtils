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
using VRCTools.World.SynchronizedValues;
using VRCTools.World.SynchronizedValues.UI;
using UnityEditor;

namespace VRCTools.World.Editor.SynchronizedValues.UI {
  [CustomEditor(typeof(SynchronizedStringLabel))]
  public class SynchronizedStringLabelEditor : AbstractCustomUdonEditor {
    private SerializedProperty _synchronizedValue;

    protected override string HelpText =>
      "Displays the current value of a synchronized string using a given string template.\n\n" +
      "This script is expected to be combined with a SynchronizedString component and will match its current state.";

    private void OnEnable() {
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(SynchronizedStringLabel.synchronizedValue));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._synchronizedValue);
      var synchronizedValue = (SynchronizedString)this._synchronizedValue.objectReferenceValue;

      if (synchronizedValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified synchronized value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
    }
  }
}
