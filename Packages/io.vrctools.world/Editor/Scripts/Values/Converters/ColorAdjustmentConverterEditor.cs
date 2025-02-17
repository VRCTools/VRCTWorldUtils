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
using VRCTools.World.Editor.Utils;
using VRCTools.World.Values.Converters;

namespace VRCTools.World.Editor.Values.Converters {
  [CustomEditor(typeof(ColorAdjustmentConverter))]
  public class ColorAdjustmentConverterEditor : AbstractCustomUdonEditor {
    private SerializedProperty _source;
    private SerializedProperty _localSource;
    private SerializedProperty _synchronizedSource;

    private SerializedProperty _target;
    private SerializedProperty _localTarget;
    private SerializedProperty _synchronizedTarget;

    private SerializedProperty _hueOffset;
    private SerializedProperty _saturationOffset;
    private SerializedProperty _valueOffset;

    private void OnEnable() {
      this._source = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.source));
      this._localSource = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.localSource));
      this._synchronizedSource
        = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.synchronizedSource));

      this._target = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.target));
      this._localTarget = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.localTarget));
      this._synchronizedTarget
        = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.synchronizedTarget));

      this._hueOffset = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.hueOffset));
      this._saturationOffset = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.saturationOffset));
      this._valueOffset = this.serializedObject.FindProperty(nameof(ColorAdjustmentConverter.valueOffset));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.LabelField("Source Value", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Source"), this._source, this._localSource,
        this._synchronizedSource, "Source value is invalid or unset - Component will be disabled on startup.");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Target Value", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Target"), this._target, this._localTarget,
        this._synchronizedTarget, "Target value is invalid or unset - Component will be disabled on startup.");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Adjustment", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._hueOffset);
      EditorGUILayout.PropertyField(this._saturationOffset);
      EditorGUILayout.PropertyField(this._valueOffset);
    }
  }
}
