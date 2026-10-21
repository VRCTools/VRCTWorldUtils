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
using VRCTools.World.SynchronizedValues;

namespace VRCTools.World.Editor.SynchronizedValues
{
  using Value = SynchronizedColor;

  [CustomEditor(typeof(Value))]
  public class SynchronizedColorEditor : AbstractSynchronizedEventEmitterEditor
  {
    private SerializedProperty _defaultValue;
    private SerializedProperty _permitAlpha;
    private SerializedProperty _permitHDR;

    private bool _colorTypeFoldout;

    protected override string HelpText =>
      "Provides a synchronized color value with event capabilities.\n\n" +
      "You may use this component to create material properties which may be changed on the fly within an instance " +
      "of this world.\n\n" +
      "Please note that this script is useless on its own. You will need to combine it with either custom " +
      "scripts or one of the scripts provided by this package: \n\n" +
      " - For material properties use MaterialParameter\n" +
      "Additional compatible scripts may be available within this package.";

    protected override void OnEnable()
    {
      base.OnEnable();

      this._defaultValue = this.serializedObject.FindProperty(nameof(Value.defaultValue));
      this._permitAlpha = this.serializedObject.FindProperty(nameof(Value.permitAlpha));
      this._permitHDR = this.serializedObject.FindProperty(nameof(Value.permitHDR));
    }

    protected override void RenderInspectorGUI()
    {
      var permitAlpha = this._permitAlpha.boolValue;
      var permitHDR = this._permitHDR.boolValue;

      EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);
      this._defaultValue.colorValue = EditorGUILayout.ColorField(new GUIContent("Default Value"),
        this._defaultValue.colorValue, true, permitAlpha,
        permitHDR);

      this._colorTypeFoldout = EditorGUILayout.Foldout(this._colorTypeFoldout, "Color Type");
      if (this._colorTypeFoldout)
      {
        EditorGUI.indentLevel++;
        {
          EditorGUILayout.PropertyField(this._permitAlpha);
          EditorGUILayout.PropertyField(this._permitHDR);
        }
        EditorGUI.indentLevel--;
      }

      EditorGUILayout.Space(20);

      base.RenderInspectorGUI();
    }
  }
}
