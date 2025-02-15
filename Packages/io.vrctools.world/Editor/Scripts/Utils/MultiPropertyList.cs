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

using System;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace VRCTools.World.Editor.Utils {
  /// <summary>
  ///   Provides an abstraction around <see cref="ReorderableList" /> which permits the creation of lists based upon
  ///   multiple properties within a given object.
  ///   This implementation is primarily needed due to Udon's lack of support for nested serialized objects which would
  ///   otherwise be used to provide this functionality.
  /// </summary>
  internal class MultiPropertyList {
    private static readonly Color ODD_BACKGROUND_COLOR = new(0, 0, 0, 0.1f);
    private static readonly Color ACTIVE_BACKGROUND_COLOR = new(.75f, .75f, 1, 0.2f);
    private static readonly Color SELECTED_BACKGROUND_COLOR = new(0, 0, 1, .1f);
    private readonly SerializedProperty[] _additionalProperties;
    private readonly string[] _additionalPropertyLabels;

    private readonly string _coreLabel;
    private readonly SerializedProperty _coreProperty;

    private readonly ReorderableList _list;

    private readonly GUIContent label;

    public MultiPropertyList(
      GUIContent label,
      SerializedObject _object,
      string coreLabel,
      SerializedProperty coreProperty,
      string[] additionalPropertyLabels,
      SerializedProperty[] additionalProperties) {
      if (additionalPropertyLabels.Length != additionalProperties.Length)
        throw new ArgumentException("Number of property labels does not match number of properties");

      this.label = label;

      this._coreLabel = coreLabel;
      this._coreProperty = coreProperty;
      this._additionalPropertyLabels = additionalPropertyLabels;
      this._additionalProperties = additionalProperties;

      this._list = new ReorderableList(_object, coreProperty, false, true, true, true) {
        drawHeaderCallback = this._OnDrawHeader,
        drawElementBackgroundCallback = this._OnDrawElementBackground,
        elementHeightCallback = this._CalculateElementHeight,
        drawElementCallback = this._OnDrawElement,
        onAddCallback = this._OnAddElement,
        onRemoveCallback = this._OnRemoveElement
      };
    }

    private void _OnDrawHeader(Rect rect) {
      if (this.label == null) return;

      EditorGUI.LabelField(rect, this.label, EditorStyles.boldLabel);
    }

    private void _OnAddElement(ReorderableList _) {
      this._coreProperty.InsertArrayElementAtIndex(this._coreProperty.arraySize);
      foreach (var property in this._additionalProperties) property.InsertArrayElementAtIndex(property.arraySize);
    }

    private void _OnRemoveElement(ReorderableList _) {
      this._coreProperty.arraySize--;
      foreach (var property in this._additionalProperties) property.arraySize--;
    }

    private float _CalculateElementHeight(int index) {
      var coreProperty = this._coreProperty.GetArrayElementAtIndex(index);
      var properties = new SerializedProperty[this._additionalProperties.Length];
      for (var i = 0; i < this._additionalProperties.Length; i++)
        properties[i] = this._additionalProperties[i].GetArrayElementAtIndex(index);

      return EditorGUI.GetPropertyHeight(coreProperty) +
             properties.Sum(EditorGUI.GetPropertyHeight);
    }

    private void _OnDrawElementBackground(Rect rect, int index, bool isActive, bool isFocused) {
      if (isFocused || isActive) {
        var color = isFocused ? SELECTED_BACKGROUND_COLOR : ACTIVE_BACKGROUND_COLOR;
        EditorGUI.DrawRect(rect, color);
        return;
      }

      if (index % 2 == 0) return;

      EditorGUI.DrawRect(rect, ODD_BACKGROUND_COLOR);
    }

    private void _OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
      var coreProperty = this._coreProperty.GetArrayElementAtIndex(index);
      var propertyLabels = new string[this._additionalProperties.Length];
      var properties = new SerializedProperty[this._additionalProperties.Length];
      for (var i = 0; i < this._additionalProperties.Length; i++) {
        propertyLabels[i] = this._additionalPropertyLabels[i];
        properties[i] = this._additionalProperties[i].GetArrayElementAtIndex(index);
      }

      EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), coreProperty,
        new GUIContent(this._coreLabel));
      rect.y += EditorGUIUtility.singleLineHeight;
      for (var i = 0; i < properties.Length; ++i) {
        var label = propertyLabels[i];
        var property = properties[i];

        EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), property,
          new GUIContent(label));
        rect.y += EditorGUIUtility.singleLineHeight;
      }
    }

    public void DoLayout() { this._list.DoLayoutList(); }
  }
}
