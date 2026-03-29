using System;
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRCTools.World.Utils;
using VRCTools.World.Values;
using ValueType = VRCTools.World.Values.ValueType;

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

    public static string GetTargetDisplayValue(
      ValueType type,
      SerializedProperty localValue,
      SerializedProperty synchronizedValue) {
      switch (type) {
        case ValueType.LOCAL:
          return GetObjectDisplayName(localValue);
        case ValueType.SYNCHRONIZED:
          return GetObjectDisplayName(synchronizedValue);
        default:
          throw new ArgumentOutOfRangeException(nameof(type), type, null);
      }
    }

    public static string GetTargetDisplayValue(
      SerializedProperty type,
      SerializedProperty localValue,
      SerializedProperty synchronizedValue) {
      var typeValue = (ValueType)type.enumValueIndex;
      return GetTargetDisplayValue(typeValue, localValue, synchronizedValue);
    }

    public static string GetSourceDisplayValue(
      ValueSource source,
      string staticValue,
      SerializedProperty localValue,
      SerializedProperty synchronizedValue) {
      switch (source) {
        case ValueSource.STATIC:
          return staticValue;
        case ValueSource.LOCAL:
          return GetObjectDisplayName(localValue);
        case ValueSource.SYNCHRONIZED:
          return GetObjectDisplayName(synchronizedValue);
        default:
          throw new ArgumentOutOfRangeException(nameof(source), source, null);
      }
    }

    public static string GetSourceDisplayValue(
      SerializedProperty source,
      string staticValue,
      SerializedProperty localValue,
      SerializedProperty synchronizedValue) {
      var sourceValue = (ValueSource)source.enumValueIndex;
      return GetSourceDisplayValue(sourceValue, staticValue, localValue, synchronizedValue);
    }

    public static string GetObjectDisplayName(SerializedProperty property) {
      var obj = property.objectReferenceValue;
      if (obj == null) {
        return "null";
      }

      if (obj is UdonSharpBehaviour) {
        return $"\"{obj.name}\"";
      }

      if (obj is Component) {
        return $"\"{obj.name}\"";
      }

      return "invalid";
    }
  }
}
