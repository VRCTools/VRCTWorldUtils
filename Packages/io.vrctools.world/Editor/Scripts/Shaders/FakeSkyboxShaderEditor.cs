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
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;

namespace VRCTools.World.Editor.Shaders {
  public class FakeSkyboxShaderEditor : AbstractCustomShaderEditor {
    protected override string HelpText =>
      "Emulates the look of the skybox on any given geometry.\n\n" +
      "This material may be used to create the effect of being able to see the sky without having to expose the actual " +
      "skybox to players by removing geometry.";

    protected override void RenderInspectorGUI(MaterialEditor me, MaterialProperty[] properties) {
      var mainTex = FindProperty("_MainTex", properties);
      var tint = FindProperty("_Tint", properties);
      var exposure = FindProperty("_Exposure", properties);
      var rotation = FindProperty("_Rotation", properties);

      me.ShaderProperty(mainTex, "Cubemap (HDR)");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Adjustments", EditorStyles.boldLabel);
      me.ShaderProperty(exposure, "Exposure");
      me.ShaderProperty(tint, "Tint");
      me.ShaderProperty(rotation, "Rotation");
    }

    protected override void _RenderGenericProperties(MaterialEditor me, MaterialProperty[] properties) {
      var zWrite = FindProperty("_ZWrite", properties);
      var zTest = FindProperty("_ZTest", properties);
      var culling = FindProperty("_Culling", properties);

      me.ShaderProperty(zWrite, "ZWrite");
      me.ShaderProperty(zTest, "ZTest");
      me.ShaderProperty(culling, "Culling");

      EditorGUILayout.Space(5);
      base._RenderGenericProperties(me, properties);
    }
  }
}
