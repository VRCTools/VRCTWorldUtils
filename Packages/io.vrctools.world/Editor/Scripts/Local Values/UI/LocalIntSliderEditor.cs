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
using UnityEngine.UI;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.LocalValues;
using VRCTools.World.LocalValues.UI;

namespace VRCTools.World.Editor.LocalValues.UI {
  [CustomEditor(typeof(LocalIntSlider))]
  public class LocalIntSliderEditor : AbstractCustomUdonEditor {
    private SerializedProperty _enableMapping;
    private SerializedProperty _localValue;
    private SerializedProperty _mappingLowerBound;
    private SerializedProperty _mappingUpperBound;
    private SerializedProperty _multiplier;

    protected override string HelpText =>
      "Provides a slider which synchronizes its value to a local int component.\n\n" +
      "This script is meant to be used together with a LocalInt component.";

    private void OnEnable() {
      this._localValue = this.serializedObject.FindProperty(nameof(LocalIntSlider.localValue));

      this._enableMapping = this.serializedObject.FindProperty(nameof(LocalIntSlider.enableMapping));
      this._mappingLowerBound = this.serializedObject.FindProperty(nameof(LocalIntSlider.mappingLowerBound));
      this._mappingUpperBound = this.serializedObject.FindProperty(nameof(LocalIntSlider.mappingUpperBound));
      this._multiplier = this.serializedObject.FindProperty(nameof(LocalIntSlider.multiplier));
    }

    protected override void RenderInspectorGUI() {
      var intSlider = this.target as LocalIntSlider;
      var slider = intSlider.gameObject.GetComponent<Slider>();

      EditorGUILayout.PropertyField(this._localValue);
      var localValue = (LocalInt)this._localValue.objectReferenceValue;

      if (localValue != null) {
        if (!localValue.restrictValue)
          EditorGUILayout.HelpBox(
            "Specified local value has no bounds set.\n\n" +
            "Sliders operate within a pre-defined range of values meaning this component may de-synchronize if the " +
            "local value is set to an out-of-range value by another component.\n\n" +
            "It is generally recommended to restrict local value when used together with slider components in " +
            "order to avoid issues.",
            MessageType.Warning
          );
        else if (slider != null && (int)slider.minValue != localValue.restrictedMinimumValue &&
                 (int)slider.maxValue != localValue.restrictedMaximumValue && !intSlider.enableMapping)
          EditorGUILayout.HelpBox(
            "Slider range does not match local value bounds.\n\n" +
            "Sliders operate within a pre-defined range of values meaning this component may de-synchronized if the " +
            "local value is set to an out-of-range value by another component\n\n" +
            "It is generally recommended to set the slider range to the same range as the local value or enable" +
            "mapping between the differing bounds in order to avoid issues",
            MessageType.Warning);
      } else {
        EditorGUILayout.HelpBox(
          "Invalid or unspecified local value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
      }

      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);

      EditorGUILayout.PropertyField(this._enableMapping);
      EditorGUI.BeginDisabledGroup(!this._enableMapping.boolValue);
      {
        CustomEditorUtility.DrawPropertiesSideBySide(new GUIContent("Range"), this._mappingLowerBound,
          this._mappingUpperBound);
        CustomEditorUtility.ValidateIntBounds(this._mappingLowerBound, this._mappingUpperBound);

        if (!localValue.restrictValue && this._enableMapping.boolValue)
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
