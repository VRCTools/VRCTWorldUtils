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

using System;
using UnityEditor;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Values.Converters;

namespace VRCTools.World.Editor.Values.Converters
{
  [CustomEditor(typeof(Vector3ComponentConverter))]
  public class Vector3ComponentConverterEditor : AbstractCustomUdonEditor
  {
    private SerializedProperty _output;
    private SerializedProperty _localOutput;
    private SerializedProperty _synchronizedOutput;

    private SerializedProperty _a;
    private SerializedProperty _staticA;
    private SerializedProperty _localA;
    private SerializedProperty _synchronizedA;

    private SerializedProperty _b;
    private SerializedProperty _staticB;
    private SerializedProperty _localB;
    private SerializedProperty _synchronizedB;

    private SerializedProperty _c;
    private SerializedProperty _staticC;
    private SerializedProperty _localC;
    private SerializedProperty _synchronizedC;

    private void OnEnable()
    {
      this._output = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.output));
      this._localOutput = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.localOutput));
      this._synchronizedOutput = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.synchronizedOutput));

      this._a = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.a));
      this._staticA = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.staticA));
      this._localA = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.localA));
      this._synchronizedA = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.synchronizedA));

      this._b = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.b));
      this._staticB = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.staticB));
      this._localB = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.localB));
      this._synchronizedB = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.synchronizedB));

      this._c = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.c));
      this._staticC = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.staticC));
      this._localC = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.localC));
      this._synchronizedC = this.serializedObject.FindProperty(nameof(Vector3ComponentConverter.synchronizedC));
    }

    protected override void RenderInspectorGUI()
    {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Output"), this._output, this._localOutput, this._synchronizedOutput, "Output value is invalid or unset - Component will be disabled on start");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("1st Component", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._a, this._staticA, this._localA, this._synchronizedA, "Component value is invalid or unset - It will be set to zero");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("2nd Component", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._b, this._staticB, this._localB, this._synchronizedB, "Component value is invalid or unset - It will be set to zero");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("3rd Component", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._c, this._staticC, this._localC, this._synchronizedC, "Component value is invalid or unset - It will be set to zero");
    }
  }
}
