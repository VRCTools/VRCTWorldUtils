﻿// Copyright 2025 .start <https://dotstart.tv>
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
using UnityEditorInternal;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.LocalValues.Applicators;

namespace VRCTools.World.Editor.LocalValues.Applicators {
  using Applicator = LocalObjectToggleApplicator;

  [CustomEditor(typeof(Applicator))]
  public class LocalObjectToggleApplicatorEditor : AbstractCustomUdonEditor {
    private SerializedProperty _localValue;

    private SerializedProperty _targets;

    private SerializedProperty _invert;

    private ReorderableList _targetList;

    private void OnEnable() {
      this._localValue
        = this.serializedObject.FindProperty(nameof(Applicator.localValue));

      this._targets = this.serializedObject.FindProperty(nameof(Applicator.targets));

      this._invert = this.serializedObject.FindProperty(nameof(Applicator.invert));

      this._targetList = new ReorderableList(this.serializedObject, this._targets, true, true, true, true);
      this._targetList.drawHeaderCallback = this._OnDrawHeader;
      this._targetList.elementHeightCallback = this._CalculateElementHeight;
      this._targetList.drawElementCallback = this._OnDrawElement;
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._localValue);
      if (this._localValue.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified local value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);

      EditorGUILayout.Space(20);

      this._targetList.DoLayoutList();

      EditorGUILayout.LabelField("Advanced Settings");
      EditorGUILayout.PropertyField(this._invert);
    }

    private float _CalculateElementHeight(int index) {
      var element = this._targets.GetArrayElementAtIndex(index);
      return EditorGUI.GetPropertyHeight(element);
    }

    private void _OnDrawHeader(Rect rect) { EditorGUI.LabelField(rect, "Targets", EditorStyles.boldLabel); }

    private void _OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
      var element = this._targets.GetArrayElementAtIndex(index);
      EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), element);
    }
  }
}
