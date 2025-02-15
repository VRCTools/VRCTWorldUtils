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
using VRCTools.World.Editor.Utils;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.SynchronizedValues.UI;
using UnityEditor;
using UnityEngine;

namespace VRCTools.World.Editor.SynchronizedValues.UI {
  [CustomEditor(typeof(SynchronizedIntLabel))]
  public class SynchronizedIntLabelEditor : AbstractCustomUdonEditor {
    private SerializedProperty _enableMapping;
    private SerializedProperty _mappingLowerBound;
    private SerializedProperty _mappingUpperBound;
    private SerializedProperty _multiplier;
    private SerializedProperty _synchronizedValue;

    protected override string HelpText =>
      "Displays the value of a synchronized int.\n\n" +
      "This script is meant to be used together with a SynchronizedInt component.";

    private void OnEnable() {
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(SynchronizedIntLabel.synchronizedValue));

      this._enableMapping = this.serializedObject.FindProperty(nameof(SynchronizedIntLabel.enableMapping));
      this._mappingLowerBound = this.serializedObject.FindProperty(nameof(SynchronizedIntLabel.mappingLowerBound));
      this._mappingUpperBound = this.serializedObject.FindProperty(nameof(SynchronizedIntLabel.mappingUpperBound));
      this._multiplier = this.serializedObject.FindProperty(nameof(SynchronizedIntLabel.multiplier));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._synchronizedValue);
      var synchronizedValue = (SynchronizedInt)this._synchronizedValue.objectReferenceValue;
      if (synchronizedValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified synchronized value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._enableMapping);
      EditorGUI.BeginDisabledGroup(!this._enableMapping.boolValue);
      {
        CustomEditorUtility.DrawPropertiesSideBySide(new GUIContent("Range"), this._mappingLowerBound,
          this._mappingUpperBound);
        CustomEditorUtility.ValidateIntBounds(this._mappingLowerBound, this._mappingUpperBound);

        if (this._enableMapping.boolValue && synchronizedValue != null && !synchronizedValue.restrictValue)
          EditorGUILayout.HelpBox(
            "Value does not specify a value range\n\n" +
            "Mapping will not be applied as no source range has been defined.",
            MessageType.Warning
          );
      }
      EditorGUI.EndDisabledGroup();
      EditorGUILayout.PropertyField(this._multiplier);
    }
  }
}
