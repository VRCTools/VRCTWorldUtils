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
using UnityEditorInternal;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.Editor.Values.Applicators {
  using Applicator = ObjectToggleApplicator;

  [CustomEditor(typeof(Applicator))]
  public class ObjectToggleApplicatorEditor : AbstractCustomUdonEditor {
    private SerializedProperty _invert;
    private SerializedProperty _localValue;
    private SerializedProperty _source;
    private SerializedProperty _synchronizedValue;

    private ReorderableList _targetList;

    private SerializedProperty _targets;

    private void OnEnable() {
      this._source = this.serializedObject.FindProperty(nameof(Applicator.source));
      this._localValue
        = this.serializedObject.FindProperty(nameof(Applicator.localValue));
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(Applicator.synchronizedValue));

      this._targets = this.serializedObject.FindProperty(nameof(Applicator.targets));

      this._invert = this.serializedObject.FindProperty(nameof(Applicator.invert));

      this._targetList = new ReorderableList(this.serializedObject, this._targets, true, true, true, true) {
        drawHeaderCallback = this._OnDrawHeader,
        elementHeightCallback = this._CalculateElementHeight,
        drawElementCallback = this._OnDrawElement
      };
    }

    protected override void RenderInspectorGUI() {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Source"), this._source, this._localValue,
        this._synchronizedValue, "Invalid or unspecified local value reference.\n\n" +
                                 "This component will disable itself upon startup.");

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
