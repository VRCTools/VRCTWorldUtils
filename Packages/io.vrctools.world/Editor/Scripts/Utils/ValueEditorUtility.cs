// Copyright 2025-2026 .start <https://dotstart.tv>
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
using UdonSharp;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.Utils;
using VRCTools.World.Values;

namespace VRCTools.World.Editor.Utils
{
  public static class ValueEditorUtility
  {
    private const string StateLabel = "Current State";

    public static void DrawSourceProperty(
      GUIContent label,
      SerializedProperty sourceProperty,
      SerializedProperty staticProperty,
      SerializedProperty localProperty,
      SerializedProperty synchronizedProperty,
      string unsetErrorMessage)
    {
      EditorGUILayout.PropertyField(sourceProperty, new GUIContent("Type"));
      var s = (ValueSource)sourceProperty.enumValueIndex;

      SerializedProperty property = null;
      switch (s)
      {
        case ValueSource.Static:
          property = staticProperty;
          break;
        case ValueSource.Local:
          property = localProperty;
          break;
        case ValueSource.Synchronized:
          property = synchronizedProperty;
          break;
      }

      EditorGUILayout.PropertyField(property, label);
      if (s != ValueSource.Static && !ValueUtility.IsValid(s, localProperty.objectReferenceValue,
            synchronizedProperty.objectReferenceValue))
        EditorGUILayout.HelpBox(unsetErrorMessage, MessageType.Error);
    }

    public static void DrawSourceProperty(
      GUIContent label,
      SerializedProperty sourceProperty,
      SerializedProperty localProperty,
      SerializedProperty synchronizedProperty,
      string unsetErrorMessage)
    {
      EditorGUILayout.PropertyField(sourceProperty, new GUIContent("Type"));
      var s = (ValueKind)sourceProperty.enumValueIndex;

      SerializedProperty property = null;
      switch (s)
      {
        case ValueKind.Local:
          property = localProperty;
          break;
        case ValueKind.Synchronized:
          property = synchronizedProperty;
          break;
      }

      EditorGUILayout.PropertyField(property, label);
      if (!ValueUtility.IsValid(s, localProperty.objectReferenceValue, synchronizedProperty.objectReferenceValue))
        EditorGUILayout.HelpBox(unsetErrorMessage, MessageType.Error);
    }

    public static string GetTargetDisplayValue(
      ValueKind kind,
      SerializedProperty localValue,
      SerializedProperty synchronizedValue)
    {
      switch (kind)
      {
        case ValueKind.Local:
          return GetObjectDisplayName(localValue);
        case ValueKind.Synchronized:
          return GetObjectDisplayName(synchronizedValue);
        default:
          throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
      }
    }

    public static string GetTargetDisplayValue(
      SerializedProperty type,
      SerializedProperty localValue,
      SerializedProperty synchronizedValue)
    {
      var typeValue = (ValueKind)type.enumValueIndex;
      return GetTargetDisplayValue(typeValue, localValue, synchronizedValue);
    }

    public static string GetSourceDisplayValue(
      ValueSource source,
      string staticValue,
      SerializedProperty localValue,
      SerializedProperty synchronizedValue)
    {
      switch (source)
      {
        case ValueSource.Static:
          return staticValue;
        case ValueSource.Local:
          return GetObjectDisplayName(localValue);
        case ValueSource.Synchronized:
          return GetObjectDisplayName(synchronizedValue);
        default:
          throw new ArgumentOutOfRangeException(nameof(source), source, null);
      }
    }

    public static string GetSourceDisplayValue(
      SerializedProperty source,
      string staticValue,
      SerializedProperty localValue,
      SerializedProperty synchronizedValue)
    {
      var sourceValue = (ValueSource)source.enumValueIndex;
      return GetSourceDisplayValue(sourceValue, staticValue, localValue, synchronizedValue);
    }

    public static string GetObjectDisplayName(SerializedProperty property)
    {
      var obj = property.objectReferenceValue;
      if (obj == null)
      {
        return "null";
      }

      if (obj is UdonSharpBehaviour)
      {
        return $"\"{obj.name}\"";
      }

      if (obj is Component)
      {
        return $"\"{obj.name}\"";
      }

      return "invalid";
    }

    public static void DrawRuntimeValue(object value)
    {
      EditorGUI.BeginDisabledGroup(true);
      switch (value)
      {
        case null: EditorGUILayout.LabelField(StateLabel, "null"); break;
        case bool b: EditorGUILayout.Toggle(StateLabel, b); break;
        case int i: EditorGUILayout.IntField(StateLabel, i); break;
        case float f: EditorGUILayout.FloatField(StateLabel, f); break;
        case string s: EditorGUILayout.TextField(StateLabel, s); break;
        case Color c: EditorGUILayout.ColorField(StateLabel, c); break;
        case Vector3 v: EditorGUILayout.Vector3Field(StateLabel, v); break;
        case VRCUrl url: EditorGUILayout.TextField(StateLabel, url.Get()); break;
        case Texture2D tex: EditorGUILayout.ObjectField(StateLabel, tex, typeof(Texture2D), false); break;
        default: EditorGUILayout.LabelField(StateLabel, value.ToString()); break;
      }
      EditorGUI.EndDisabledGroup();
    }
  }
}
