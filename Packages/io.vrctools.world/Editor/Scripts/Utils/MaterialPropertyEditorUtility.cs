using UnityEditor;
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.Editor.Utils {
  public static class MaterialPropertyEditorUtility {
    public static void DrawTarget(SerializedProperty target, SerializedProperty material, SerializedProperty renderer) {
      EditorGUILayout.PropertyField(target);
      var t = (MaterialPropertyTarget)target.enumValueIndex;

      switch (t) {
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
