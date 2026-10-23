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

using UdonSharp;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;
using VRCTools.Common.Attributes;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;

namespace VRCTools.World.Editor.LocalValues
{
  public abstract class AbstractLocalValueEditor : AbstractEventEmitterEditor
  {
    protected virtual string _defaultValuePropertyName => "defaultValue";
    protected virtual string _stateVariableName => "_state";

    protected SerializedProperty _defaultValue;

    [MustCallBase]
    protected override void OnEnable()
    {
      base.OnEnable();

      this._defaultValue = this.serializedObject.FindProperty(this._defaultValuePropertyName);
    }

    public override bool RequiresConstantRepaint()
    {
      return Application.isPlaying && this._stateVariableName != null;
    }

    protected override void RenderInspectorGUI()
    {
      this.RenderValueProperties();
      EditorGUILayout.Space(20);

      base.RenderInspectorGUI();

      if (!Application.isPlaying || this._stateVariableName == null)
      {
        return;
      }

      this.DrawRuntimeValue();
    }

    protected virtual void RenderValueProperties()
    {
     this.RenderDefaults();
    }

    protected virtual void RenderDefaults()
    {
      EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._defaultValue);
    }

    protected virtual void DrawRuntimeValue()
    {
      var behaviour = this.target as UdonSharpBehaviour;
      if (behaviour == null)
      {
        return;
      }

      var udon = UdonSharpEditorUtility.GetBackingUdonBehaviour(behaviour);
      if (udon == null || !udon.TryGetProgramVariable(this._stateVariableName, out var state))
      {
        return;
      }

      EditorGUILayout.Space(20);
      EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
      ValueEditorUtility.DrawRuntimeValue(state);
    }
  }
}
