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
  [CustomEditor(typeof(GradientConverter))]
  public class GradientConverterEditor : AbstractCustomUdonEditor
  {
    private SerializedProperty _input;
    private SerializedProperty _localInput;
    private SerializedProperty _synchronizedInput;

    private SerializedProperty _gradient;

    private SerializedProperty _output;
    private SerializedProperty _localOutput;
    private SerializedProperty _synchronizedOutput;

    private void OnEnable()
    {
      this._input = this.serializedObject.FindProperty(nameof(GradientConverter.input));
      this._localInput = this.serializedObject.FindProperty(nameof(GradientConverter.localInput));
      this._synchronizedInput = this.serializedObject.FindProperty(nameof(GradientConverter.synchronizedInput));

      this._gradient = this.serializedObject.FindProperty(nameof(GradientConverter.gradient));

      this._output = this.serializedObject.FindProperty(nameof(GradientConverter.output));
      this._localOutput = this.serializedObject.FindProperty(nameof(GradientConverter.localOutput));
      this._synchronizedOutput = this.serializedObject.FindProperty(nameof(GradientConverter.synchronizedOutput));
    }

    protected override void RenderInspectorGUI()
    {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Input"), this._input, this._localInput,
        this._synchronizedInput, "Input unset - Converter will not run");
      EditorGUILayout.Space(20);

      EditorGUILayout.PropertyField(this._gradient);
      EditorGUILayout.Space(20);

      ValueEditorUtility.DrawSourceProperty(new GUIContent("Output"), this._output, this._localOutput,
        this._synchronizedOutput, "Output unset - Converter will not run");
    }
  }
}
