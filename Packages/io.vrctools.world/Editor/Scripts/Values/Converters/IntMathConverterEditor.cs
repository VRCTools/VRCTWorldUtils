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
using VRCTools.World.Values;
using VRCTools.World.Values.Converters;

namespace VRCTools.World.Editor.Values.Converters
{
  using Converter = IntMathConverter;

  [CustomEditor(typeof(Converter))]
  public class IntMathConverterEditor : AbstractCustomUdonEditor
  {
    private SerializedProperty _target;
    private SerializedProperty _localTarget;
    private SerializedProperty _synchronizedTarget;

    private SerializedProperty _operation;

    private SerializedProperty _sourceA;
    private SerializedProperty _staticValueA;
    private SerializedProperty _localValueA;
    private SerializedProperty _synchronizedValueA;

    private SerializedProperty _sourceB;
    private SerializedProperty _staticValueB;
    private SerializedProperty _localValueB;
    private SerializedProperty _synchronizedValueB;

    protected override string HelpText => "Performs simple math operations on two values and writes the result to a" +
                                          "given target.";

    private void OnEnable()
    {
      this._target = this.serializedObject.FindProperty(nameof(Converter.target));
      this._localTarget = this.serializedObject.FindProperty(nameof(Converter.localTarget));
      this._synchronizedTarget = this.serializedObject.FindProperty(nameof(Converter.synchronizedTarget));

      this._operation = this.serializedObject.FindProperty(nameof(Converter.operation));

      this._sourceA = this.serializedObject.FindProperty(nameof(Converter.sourceA));
      this._staticValueA = this.serializedObject.FindProperty(nameof(Converter.staticValueA));
      this._localValueA = this.serializedObject.FindProperty(nameof(Converter.localValueA));
      this._synchronizedValueA = this.serializedObject.FindProperty(nameof(Converter.synchronizedValueA));

      this._sourceB = this.serializedObject.FindProperty(nameof(Converter.sourceB));
      this._staticValueB = this.serializedObject.FindProperty(nameof(Converter.staticValueB));
      this._localValueB = this.serializedObject.FindProperty(nameof(Converter.localValueB));
      this._synchronizedValueB = this.serializedObject.FindProperty(nameof(Converter.synchronizedValueB));
    }

    protected override void RenderInspectorGUI()
    {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("target"), this._target, this._localTarget,
        this._synchronizedTarget, "Target value is invalid or unset - Component will be disabled on start");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Math", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._operation);

      var operation = (IntegralMathOperation)this._operation.enumValueIndex;

      var formula = ValueEditorUtility.GetTargetDisplayValue(this._target, this._localTarget, this._synchronizedTarget);
      formula += " = ";

      formula += ValueEditorUtility.GetSourceDisplayValue(this._sourceA, $"{this._staticValueA.intValue}",
        this._localValueA, this._synchronizedValueA);

      formula += " ";
      switch (operation)
      {
        case IntegralMathOperation.ADD:
          formula += "+";
          break;
        case IntegralMathOperation.SUBTRACT:
          formula += "-";
          break;
        case IntegralMathOperation.MULTIPLY:
          formula += "*";
          break;
        case IntegralMathOperation.DIVIDE:
          formula += "/";
          break;
        case IntegralMathOperation.REM:
          formula += "%";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      formula += " ";

      formula += ValueEditorUtility.GetSourceDisplayValue(this._sourceB, $"{this._staticValueB.intValue}",
        this._localValueB, this._synchronizedValueB);
      EditorGUILayout.LabelField("Formula", formula);

      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("A", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Value"), this._sourceA, this._staticValueA,
        this._localValueA, this._synchronizedValueA,
        "Parameter is invalid or unset - It will be set to zero");

      EditorGUILayout.Space(20);
      EditorGUILayout.LabelField("B", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Value"), this._sourceB, this._staticValueB,
        this._localValueB, this._synchronizedValueB,
        "Parameter is invalid or unset - It will be set to zero");
    }
  }
}
