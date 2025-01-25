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
  using ColorPicker = SynchronizedColorPicker;

  [CustomEditor(typeof(ColorPicker))]
  public class SynchronizedColorPickerEditor : AbstractColorPickerEditor {
    private SerializedProperty _synchronizedValue;

    protected override string HelpText =>
      "Provides a visual color picker which may be used to adjust synchronized color values within your world on the " +
      "fly.\n\n" +
      "This script is expected to be paired with a SynchronizedColor component.";

    protected override void OnEnable() {
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(ColorPicker.synchronizedValue));

      base.OnEnable();
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._synchronizedValue);
      if (this._synchronizedValue.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified synchronized value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);

      EditorGUILayout.Space(20);

      base.RenderInspectorGUI();
    }
  }
}
