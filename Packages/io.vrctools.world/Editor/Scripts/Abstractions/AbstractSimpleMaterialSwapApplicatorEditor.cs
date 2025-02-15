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
using VRCTools.World.Abstractions;
using VRCTools.World.Editor.Utils;

namespace VRCTools.World.Editor.Abstractions {
  using Applicator = AbstractSimpleMaterialSwapApplicator;

  public abstract class AbstractSimpleMaterialSwapApplicatorEditor : AbstractCustomUdonEditor {
    private SerializedProperty _renderers;
    private SerializedProperty _slotIndices;
    private SerializedProperty _replacementMaterials;

    private SerializedProperty _invert;

    private MultiPropertyList _rendererList;

    protected virtual void OnEnable() {
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
      this._rendererList.DoLayout();
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Advanced Settings");
      EditorGUILayout.PropertyField(this._invert);
    }
  }
}
