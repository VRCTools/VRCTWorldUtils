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
using VRCTools.World.Values.Converters;

namespace VRCTools.World.Editor.Values.Converters {
  [CustomEditor(typeof(BooleanOperationConverter))]
  public class BooleanOperationConverterEditor : AbstractCustomUdonEditor {
    public SerializedProperty _useSynchronizedTarget;
    public SerializedProperty _localTarget;
    public SerializedProperty _synchronizedTarget;

    public SerializedProperty _operation;

    public SerializedProperty _sourceAlpha;
    public SerializedProperty _staticAlphaValue;
    public SerializedProperty _localAlphaValue;
    public SerializedProperty _synchronizedAlphaValue;

    public SerializedProperty _sourceBeta;
    public SerializedProperty _staticBetaValue;
    public SerializedProperty _localBetaValue;
    public SerializedProperty _synchronizedBetaValue;

    private void OnEnable() {
      this._useSynchronizedTarget
        = this.serializedObject.FindProperty(nameof(BooleanOperationConverter.useSynchronizedTarget));
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
      EditorGUILayout.PropertyField(this._useSynchronizedTarget);

      var useSynchronizedTarget = this._useSynchronizedTarget.boolValue;
      EditorGUILayout.PropertyField(useSynchronizedTarget ? this._synchronizedTarget : this._localTarget);
      if ((useSynchronizedTarget && !Utilities.IsValid(this._synchronizedTarget.objectReferenceValue)) ||
          (!useSynchronizedTarget && !Utilities.IsValid(this._localTarget.objectReferenceValue))) {
        EditorGUILayout.HelpBox("Target value is invalid or unset - Component will be disabled on start",
          MessageType.Error);
      }

      EditorGUILayout.PropertyField(this._operation);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("A", EditorStyles.boldLabel);
      _DrawProperty(this._sourceAlpha, this._staticAlphaValue, this._localAlphaValue, this._synchronizedAlphaValue);

      var operation = (BooleanOperation)this._operation.enumValueIndex;
      if (operation == BooleanOperation.NOT) return;

      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("B", EditorStyles.boldLabel);
      _DrawProperty(this._sourceBeta, this._staticBetaValue, this._localBetaValue, this._synchronizedBetaValue);
    }

    private static void _DrawProperty(
      SerializedProperty source,
      SerializedProperty staticProperty,
      SerializedProperty localProperty,
      SerializedProperty synchronizedProperty) {
      EditorGUILayout.PropertyField(source);
      var s = (ValueSource)source.enumValueIndex;

      SerializedProperty property = null;
      switch (s) {
        case ValueSource.STATIC:
          property = staticProperty;
          break;
        case ValueSource.LOCAL:
          property = localProperty;
          break;
        case ValueSource.SYNCHRONIZED:
          property = synchronizedProperty;
          break;
      }

      EditorGUILayout.PropertyField(property, new GUIContent("Source"));
      if (s != ValueSource.STATIC && !Utilities.IsValid(property.objectReferenceValue)) {
        EditorGUILayout.HelpBox("Source value is invalid or unset - Component will be set to false", MessageType.Error);
      }
    }
  }
}
