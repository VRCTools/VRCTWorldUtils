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
using VRCTools.World.LocalValues;

namespace VRCTools.World.Editor.LocalValues {
  using Value = LocalBoolean;

  [CustomEditor(typeof(Value))]
  public class LocalBooleanEditor : AbstractCustomUdonEditor {
    private SerializedProperty _defaultValue;

    protected override string HelpText =>
      "Provides a local boolean value with event capabilities.\n\n" +
      "You may use this component to create objects, animations or material properties which may be toggled on/off" +
      " on the fly within an instance of this world.\n\n" +
      "Please note that this script is useless on its own. You will need to combine it with either custom scripts or" +
      " one of the toggle scripts provided by this package: \n\n" +
      " - For animators use LocalAnimatorApplicator\n\n" +
      " - For simple object toggles use LocalObjectApplicator\n" +
      " - For material replacements use LocalMaterialSwapApplicator\n" +
      " - For material replacements use LocalMaterialPropertyApplicator\n" +
      "Additional compatible scripts may be available within this package.";

    private void OnEnable() { this._defaultValue = this.serializedObject.FindProperty(nameof(Value.defaultValue)); }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._defaultValue);
    }
  }
}
