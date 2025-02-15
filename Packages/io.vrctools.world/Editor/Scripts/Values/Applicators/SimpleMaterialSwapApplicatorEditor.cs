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
using VRCTools.World.Editor.Utils;
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.Editor.Values.Applicators {
  using Applicator = SimpleMaterialSwapApplicator;

  [CustomEditor(typeof(Applicator))]
  public class SimpleMaterialSwapApplicatorEditor : AbstractCustomUdonEditor {
    private SerializedProperty _invert;
    private SerializedProperty _localValue;

    private MultiPropertyList _rendererList;

    private SerializedProperty _renderers;
    private SerializedProperty _replacementMaterials;
    private SerializedProperty _slotIndices;
    private SerializedProperty _source;
    private SerializedProperty _synchronizedValue;

    private void OnEnable() {
      this._source = this.serializedObject.FindProperty(nameof(Applicator.source));
      this._localValue = this.serializedObject.FindProperty(nameof(Applicator.localValue));
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(Applicator.synchronizedValue));

      this._renderers = this.serializedObject.FindProperty(nameof(Applicator.renderers));
      this._slotIndices = this.serializedObject.FindProperty(nameof(Applicator.slotIndices));
      this._replacementMaterials = this.serializedObject.FindProperty(nameof(Applicator.replacementMaterials));

      this._invert = this.serializedObject.FindProperty(nameof(Applicator.invert));

      this._rendererList = new MultiPropertyList(
        new GUIContent("Swaps"),
        this.serializedObject,
        "Renderer",
        this._renderers,
        new[] { "Index", "Replacement" },
        new[] { this._slotIndices, this._replacementMaterials });
    }

    protected override void RenderInspectorGUI() {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Source"), this._source, this._localValue,
        this._synchronizedValue, "Invalid or unspecified local value reference.\n\n" +
                                 "This component will disable itself upon startup.");

      EditorGUILayout.Space(20);

      this._rendererList.DoLayout();
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Advanced Settings");
      EditorGUILayout.PropertyField(this._invert);
    }
  }
}
