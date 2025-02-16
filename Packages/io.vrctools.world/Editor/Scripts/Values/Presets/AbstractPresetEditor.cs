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
using VRC.SDKBase;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;

namespace VRCTools.World.Editor.Values.Presets {
  public abstract class AbstractPresetEditor : AbstractCustomUdonEditor {
    private SerializedProperty _target;
    private SerializedProperty _localTarget;
    private SerializedProperty _synchronizedTarget;

    private SerializedProperty _preset;

    private void OnEnable() {
      // typically we use nameof to not cause too much trouble during refactors - this is an exception since all presets
      // share the same basic pattern which we cannot share purely due to limitations within the Udon# compiler
      this._target = this.serializedObject.FindProperty("target");
      this._localTarget = this.serializedObject.FindProperty("localTarget");
      this._synchronizedTarget = this.serializedObject.FindProperty("synchronizedTarget");

      this._preset = this.serializedObject.FindProperty("preset");
    }

    protected override void RenderInspectorGUI() {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Target"), this._target, this._localTarget,
        this._synchronizedTarget, "Target reference is invalid or unspecified - Component will be disabled on startup");
      EditorGUILayout.Space(20);
      EditorGUILayout.PropertyField(this._preset);
    }
  }
}
