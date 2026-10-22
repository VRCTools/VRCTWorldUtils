// Copyright 2026 .start <https://dotstart.tv>
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
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.Editor.Values.Applicators
{
  [CustomEditor(typeof(TransformApplicator))]
  public class TransformApplicatorEditor : AbstractCustomEditor
  {
    private SerializedProperty _positionTransformSpace;
    private SerializedProperty _positionOperation;
    private SerializedProperty _positionModifier;
    private SerializedProperty _localPositionModifier;
    private SerializedProperty _synchronizedPositionModifier;

    private SerializedProperty _rotationTransformSpace;
    private SerializedProperty _rotationOperation;
    private SerializedProperty _rotationModifier;
    private SerializedProperty _localRotationModifier;
    private SerializedProperty _synchronizedRotationModifier;

    private SerializedProperty _scaleOperation;
    private SerializedProperty _scaleModifier;
    private SerializedProperty _localScaleModifier;
    private SerializedProperty _synchronizedScaleModifier;

    private void OnEnable()
    {
      this._positionTransformSpace =
        this.serializedObject.FindProperty(nameof(TransformApplicator.positionTransformSpace));
      this._positionOperation =
        this.serializedObject.FindProperty(nameof(TransformApplicator.positionOperation));
      this._positionModifier =
        this.serializedObject.FindProperty(nameof(TransformApplicator.positionModifier));
      this._localPositionModifier =
        this.serializedObject.FindProperty(nameof(TransformApplicator.localPositionModifier));
      this._synchronizedPositionModifier =
        this.serializedObject.FindProperty(nameof(TransformApplicator.synchronizedPositionModifier));

      this._rotationTransformSpace =
        this.serializedObject.FindProperty(nameof(TransformApplicator.rotationTransformSpace));
      this._rotationOperation =
        this.serializedObject.FindProperty(nameof(TransformApplicator.rotationOperation));
      this._rotationModifier =
        this.serializedObject.FindProperty(nameof(TransformApplicator.rotationModifier));
      this._localRotationModifier =
        this.serializedObject.FindProperty(nameof(TransformApplicator.localRotationModifier));
      this._synchronizedRotationModifier =
        this.serializedObject.FindProperty(nameof(TransformApplicator.synchronizedRotationModifier));

      this._scaleOperation = this.serializedObject.FindProperty(nameof(TransformApplicator.scaleOperation));
      this._scaleModifier =
        this.serializedObject.FindProperty(nameof(TransformApplicator.scaleModifier));
      this._localScaleModifier = this.serializedObject.FindProperty(nameof(TransformApplicator.localScaleModifier));
      this._synchronizedScaleModifier =
        this.serializedObject.FindProperty(nameof(TransformApplicator.synchronizedScaleModifier));
    }

    protected override void RenderInspectorGUI()
    {
      EditorGUILayout.LabelField("Position", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._positionTransformSpace);
      EditorGUILayout.PropertyField(this._positionOperation);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Modifier"), this._positionModifier,
        this._localPositionModifier, this._synchronizedPositionModifier,
        "Modifier is unset - Position will remain unmodified");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Rotation", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._rotationTransformSpace);
      EditorGUILayout.PropertyField(this._rotationOperation);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Modifier"), this._rotationModifier,
        this._localRotationModifier, this._synchronizedRotationModifier,
        "Modifier is unset - Rotation will remain unmodified");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Scale", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._scaleOperation);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Modifier"), this._scaleModifier, this._localScaleModifier,
        this._synchronizedScaleModifier, "Modifier is unset - Scale will remain unmodified");
    }
  }
}
