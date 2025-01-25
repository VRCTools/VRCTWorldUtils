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
using VRCTools.World.LocalValues;

namespace VRCTools.World.Editor.LocalValues {
  using Value = LocalInt;

  [CustomEditor(typeof(Value))]
  public class LocalIntEditor : AbstractCustomUdonEditor {
    private SerializedProperty _defaultValue;
    private SerializedProperty _restrictedMaximumValue;
    private SerializedProperty _restrictedMinimumValue;

    private SerializedProperty _restrictValue;

    protected override string HelpText =>
      "Provides a local int value with event capabilities.\n\n" +
      "You may use this component to create animations or material properties which may be changed on the fly " +
      "within an instance of this world.\n\n" +
      "Please note that this script is useless on its own. You will need to combine it with either custom " +
      "scripts or one of the scripts provided by this package: \n\n" +
      " - For material properties use LocalMaterialPropertyApplicator\n" +
      " - For animators use LocalAnimatorApplicator\n\n" +
      "Additional compatible scripts may be available within this package.";

    private void OnEnable() {
      this._defaultValue = this.serializedObject.FindProperty(nameof(Value.defaultValue));

      this._restrictValue = this.serializedObject.FindProperty(nameof(Value.restrictValue));
      this._restrictedMinimumValue = this.serializedObject.FindProperty(nameof(Value.restrictedMinimumValue));
      this._restrictedMaximumValue = this.serializedObject.FindProperty(nameof(Value.restrictedMaximumValue));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._defaultValue);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Restrictions", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._restrictValue);
      EditorGUI.BeginDisabledGroup(!this._restrictValue.boolValue);
      {
        EditorGUILayout.PropertyField(this._restrictedMinimumValue, new GUIContent("Minimum Value"));
        EditorGUILayout.PropertyField(this._restrictedMaximumValue, new GUIContent("Maximum Value"));

        if (this._restrictedMinimumValue.intValue > this._restrictedMaximumValue.intValue)
          EditorGUILayout.HelpBox("Minimum value cannot exceed maximum value - Restriction will be ignored",
            MessageType.Error);
      }
      EditorGUI.EndDisabledGroup();
    }
  }
}
