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
using VRCTools.World.LocalValues.Applicators;

namespace VRCTools.World.Editor.LocalValues.Applicators {
  using Applicator = LocalObjectToggleApplicator;

  [CustomEditor(typeof(Applicator))]
  public class LocalObjectToggleApplicatorEditor : AbstractObjectToggleApplicatorEditor {
    private SerializedProperty _localValue;

    protected override void OnEnable() {
      this._localValue
        = this.serializedObject.FindProperty(nameof(Applicator.localValue));

      base.OnEnable();
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._localValue);
      if (this._localValue.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified local value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);

      EditorGUILayout.Space(20);

      base.RenderInspectorGUI();
    }
  }
}
