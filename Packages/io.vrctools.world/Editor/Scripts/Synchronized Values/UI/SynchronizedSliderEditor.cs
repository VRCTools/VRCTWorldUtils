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
using VRCTools.World.SynchronizedValues;
using VRCTools.World.SynchronizedValues.UI;

namespace VRCTools.World.Editor.SynchronizedValues.UI {
  [CustomEditor(typeof(SynchronizedSlider))]
  public class SynchronizedSliderEditor : AbstractCustomUdonEditor {
    private SerializedProperty _enableMapping;
    private SerializedProperty _mappingLowerBound;
    private SerializedProperty _mappingUpperBound;
    private SerializedProperty _multiplier;
    private SerializedProperty _synchronizedValue;

    protected override string HelpText =>
      "Permits the control of a synchronized float via a Slider UI component.\n\n" +
      "This script is expected to be combined with a SynchronizedFloat component and will match its current state so " +
      "long as its range matches the configuration of the SynchronizedFloat component.";

    private void OnEnable() {
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(SynchronizedSlider.synchronizedValue));

      this._enableMapping = this.serializedObject.FindProperty(nameof(SynchronizedSlider.enableMapping));
      this._mappingLowerBound = this.serializedObject.FindProperty(nameof(SynchronizedSlider.mappingLowerBound));
      this._mappingUpperBound = this.serializedObject.FindProperty(nameof(SynchronizedSlider.mappingUpperBound));
      this._multiplier = this.serializedObject.FindProperty(nameof(SynchronizedSlider.multiplier));
    }

    protected override void RenderInspectorGUI() {
      var intSlider = this.target as SynchronizedSlider;
      var slider = intSlider.gameObject.GetComponent<Slider>();

      EditorGUILayout.PropertyField(this._synchronizedValue);
      var synchronizedValue = (SynchronizedFloat)this._synchronizedValue.objectReferenceValue;

      if (synchronizedValue != null) {
        if (!synchronizedValue.restrictValue)
          EditorGUILayout.HelpBox(
            "Specified synchronized value has no bounds set.\n\n" +
            "Sliders operate within a pre-defined range of values meaning this component may de-synchronize if the " +
            "synchronized value is set to an out-of-range value by another component.\n\n" +
            "It is generally recommended to restrict synchronized value when used together with slider components in " +
            "order to avoid issues.",
            MessageType.Warning
          );
        else if (slider != null && !Mathf.Approximately(slider.minValue, synchronizedValue.restrictedMinimumValue) &&
                 !Mathf.Approximately(slider.maxValue, synchronizedValue.restrictedMaximumValue) &&
                 !intSlider.enableMapping)
          EditorGUILayout.HelpBox(
            "Slider range does not match synchronized value bounds.\n\n" +
            "Sliders operate within a pre-defined range of values meaning this component may de-synchronized if the " +
            "synchronized value is set to an out-of-range value by another component\n\n" +
            "It is generally recommended to set the slider range to the same range as the synchronized value or enable" +
            "mapping between the differing bounds in order to avoid issues",
            MessageType.Warning);
      } else {
        EditorGUILayout.HelpBox(
          "Invalid or unspecified synchronized value reference.\n\n" +
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

        if (!synchronizedValue.restrictValue && this._enableMapping.boolValue)
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
