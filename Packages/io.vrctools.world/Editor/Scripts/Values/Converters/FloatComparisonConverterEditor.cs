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
  [CustomEditor(typeof(FloatComparisonConverter))]
  public class FloatComparisonConverterEditor : AbstractCustomUdonEditor
  {
    private SerializedProperty _output;
    private SerializedProperty _localOutput;
    private SerializedProperty _synchronizedOutput;

    private SerializedProperty _a;
    private SerializedProperty _staticA;
    private SerializedProperty _localA;
    private SerializedProperty _synchronizedA;

    public SerializedProperty _operation;

    private SerializedProperty _b;
    private SerializedProperty _staticB;
    private SerializedProperty _localB;
    private SerializedProperty _synchronizedB;

    private void OnEnable()
    {
      this._output = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.output));
      this._localOutput = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.localOutput));
      this._synchronizedOutput =
        this.serializedObject.FindProperty(nameof(FloatComparisonConverter.synchronizedOutput));

      this._a = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.a));
      this._staticA = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.staticA));
      this._localA = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.localA));
      this._synchronizedA = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.synchronizedA));

      this._operation = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.operation));

      this._b = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.b));
      this._staticB = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.staticB));
      this._localB = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.localB));
      this._synchronizedB = this.serializedObject.FindProperty(nameof(FloatComparisonConverter.synchronizedB));
    }

    protected override void RenderInspectorGUI()
    {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Output"), this._output, this._localOutput,
        this._synchronizedOutput, "Output has not been set - Converter will not run");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Comparison", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._operation);

      var operation = (ComparisonOperation)this._operation.enumValueIndex;

      var formula = ValueEditorUtility.GetTargetDisplayValue(this._output, this._localOutput, this._synchronizedOutput);
      formula += " = ";

      formula += ValueEditorUtility.GetSourceDisplayValue(this._a, $"{this._staticA.floatValue:0.00}",
        this._localA, this._synchronizedA);

      formula += " ";
      switch (operation)
      {
        case ComparisonOperation.LessThan:
          formula += "<";
          break;
        case ComparisonOperation.LessThanOrEqualTo:
          formula += "<=";
          break;
        case ComparisonOperation.GreaterThan:
          formula += ">";
          break;
        case ComparisonOperation.GreaterThanOrEqualTo:
          formula += ">=";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      formula += " ";

      formula += ValueEditorUtility.GetSourceDisplayValue(this._b, $"{this._staticB.floatValue:0.00}",
        this._localB, this._synchronizedB);
      EditorGUILayout.LabelField("Formula", formula);

      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("A", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Value"), this._a, this._staticA,
        this._localA, this._synchronizedA,
        "Parameter is invalid or unset - It will be set to zero");

      EditorGUILayout.Space(20);
      EditorGUILayout.LabelField("B", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Value"), this._b, this._staticB,
        this._localB, this._synchronizedB,
        "Parameter is invalid or unset - It will be set to zero");
    }
  }
}
