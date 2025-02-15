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
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Player;

namespace VRCTools.World.Editor.Player {
  [CustomEditor(typeof(PlayerTeleportTarget))]
  public class PlayerTeleportTargetEditor : AbstractCustomUdonEditor {
    private SerializedProperty _target;

    private void OnEnable() { this._target = this.serializedObject.FindProperty(nameof(PlayerTeleportTarget.target)); }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._target);
      if (this._target.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "No target has been assigned - Teleport target will match the position and rotation of this game object.",
          MessageType.Info);
    }
  }
}
