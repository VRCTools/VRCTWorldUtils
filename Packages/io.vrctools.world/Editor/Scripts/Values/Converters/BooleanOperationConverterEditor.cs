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
using VRC.SDKBase;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Values;
using VRCTools.World.Values.Converters;

namespace VRCTools.World.Editor.Values.Converters {
  [CustomEditor(typeof(BooleanOperationConverter))]
  public class BooleanOperationConverterEditor : AbstractCustomUdonEditor {
    public SerializedProperty _target;
    public SerializedProperty _localTarget;
    public SerializedProperty _synchronizedTarget;

    public SerializedProperty _localAlphaValue;
    public SerializedProperty _localBetaValue;

    public SerializedProperty _operation;

    public SerializedProperty _sourceAlpha;

    public SerializedProperty _sourceBeta;
    public SerializedProperty _staticAlphaValue;
    public SerializedProperty _staticBetaValue;
    public SerializedProperty _synchronizedAlphaValue;
    public SerializedProperty _synchronizedBetaValue;

    private void OnEnable() {
      this._target
        = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.target));
      this._localTarget = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.localTarget));
      this._synchronizedTarget
        = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.synchronizedTarget));

      this._operation = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.operation));

      this._sourceAlpha = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.sourceAlpha));
      this._staticAlphaValue = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.staticAlphaValue));
      this._localAlphaValue = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.localAlphaValue));
      this._synchronizedAlphaValue
        = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.synchronizedAlphaValue));

      this._sourceBeta = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.sourceBeta));
      this._staticBetaValue = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.staticBetaValue));
      this._localBetaValue = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.localBetaValue));
      this._synchronizedBetaValue
        = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.synchronizedBetaValue));
    }

    protected override void RenderInspectorGUI() {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("target"), this._target, this._localTarget,
        this._synchronizedTarget, "Target value is invalid or unset - Component will be disabled on start");
      EditorGUILayout.PropertyField(this._operation);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("A", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._sourceAlpha, this._staticAlphaValue,
        this._localAlphaValue, this._synchronizedAlphaValue,
        "Component value is invalid or unset - Value will be substituted by false");

      var operation = (BooleanOperation)this._operation.enumValueIndex;
      if (operation == BooleanOperation.NOT) return;
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("B", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._sourceBeta, this._staticBetaValue,
        this._localBetaValue, this._synchronizedBetaValue,
        "Component value is invalid or unset - Value will be substituted by false");
    }
  }
}
