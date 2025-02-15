using UnityEditor;
using UnityEngine;
using VRCTools.World.Utils;
using VRCTools.World.Values;

namespace VRCTools.World.Editor.Utils {
  public static class ValueEditorUtility {
    public static void DrawSourceProperty(
      GUIContent label,
      SerializedProperty sourceProperty,
      SerializedProperty staticProperty,
      SerializedProperty localProperty,
      SerializedProperty synchronizedProperty,
      string unsetErrorMessage) {
      EditorGUILayout.PropertyField(sourceProperty, new GUIContent("Type"));
      var s = (ValueSource)sourceProperty.enumValueIndex;

      SerializedProperty property = null;
      switch (s) {
        case ValueSource.STATIC:
          property = staticProperty;
          break;
        case ValueSource.LOCAL:
          property = localProperty;
          break;
        case ValueSource.SYNCHRONIZED:
          property = synchronizedProperty;
          break;
      }

      EditorGUILayout.PropertyField(property, label);
      if (s != ValueSource.STATIC && !ValueUtility.IsValid(s, localProperty.objectReferenceValue,
            synchronizedProperty.objectReferenceValue))
        EditorGUILayout.HelpBox(unsetErrorMessage, MessageType.Error);
    }

    public static void DrawSourceProperty(
      GUIContent label,
      SerializedProperty sourceProperty,
      SerializedProperty localProperty,
      SerializedProperty synchronizedProperty,
      string unsetErrorMessage) {
      EditorGUILayout.PropertyField(sourceProperty, new GUIContent("Type"));
      var s = (ValueType)sourceProperty.enumValueIndex;

      SerializedProperty property = null;
      switch (s) {
        case ValueType.LOCAL:
          property = localProperty;
          break;
        case ValueType.SYNCHRONIZED:
          property = synchronizedProperty;
          break;
      }

      EditorGUILayout.PropertyField(property, label);
      if (!ValueUtility.IsValid(s, localProperty.objectReferenceValue, synchronizedProperty.objectReferenceValue))
        EditorGUILayout.HelpBox(unsetErrorMessage, MessageType.Error);
    }
  }
}
