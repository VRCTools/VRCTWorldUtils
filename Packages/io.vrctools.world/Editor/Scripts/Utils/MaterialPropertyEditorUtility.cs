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
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.Editor.Utils
{
  public static class MaterialPropertyEditorUtility
  {
    public static void DrawTarget(SerializedProperty target, SerializedProperty material, SerializedProperty renderer)
    {
      EditorGUILayout.PropertyField(target);
      var t = (MaterialPropertyTarget)target.enumValueIndex;

      switch (t)
      {
        case MaterialPropertyTarget.MATERIAL:
          EditorGUILayout.PropertyField(material);
          break;
        case MaterialPropertyTarget.PROPERTY_BLOCK:
          EditorGUILayout.PropertyField(renderer);
          EditorGUILayout.Space(5);
          EditorGUILayout.HelpBox(
            "Important: Material Property Blocks are only available for shaders which have been explicitly " +
            "designed to support instancing. Additionally, \"Enable GPU instancing\" must be selected within the " +
            "material. If your changes are not taking effect, you may need to apply properties directly to the " +
            "Material or switch to a different shader.",
            MessageType.Info
          );
          break;
        case MaterialPropertyTarget.GLOBAL:
          EditorGUILayout.HelpBox(
            "Note: With very few exceptions all property names must begin with \"_Udon\" in order to be applied.",
            MessageType.Warning);
          break;
      }
    }
  }
}
