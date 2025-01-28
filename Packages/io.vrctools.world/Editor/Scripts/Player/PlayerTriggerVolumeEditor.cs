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
using UnityEditor;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Player;

namespace VRCTools.World.Editor.Player {
  [CustomEditor(typeof(PlayerTriggerVolume))]
  public class PlayerTriggerVolumeEditor : AbstractCustomUdonEditor {
    private SerializedProperty _enterBehaviours;
    private SerializedProperty _enterBehaviourEventNames;
    private MultiPropertyList _enterBehaviorList;

    private SerializedProperty _exitBehaviours;
    private SerializedProperty _exitBehaviourEventNames;
    private MultiPropertyList _exitBehaviorList;

    private SerializedProperty _acceptedPlayerTypes;

    private void OnEnable() {
      this._enterBehaviours = this.serializedObject.FindProperty(nameof(PlayerTriggerVolume.enterBehaviours));
      this._enterBehaviourEventNames
        = this.serializedObject.FindProperty(nameof(PlayerTriggerVolume.enterBehaviorEventNames));

      this._exitBehaviours = this.serializedObject.FindProperty(nameof(PlayerTriggerVolume.exitBehaviours));
      this._exitBehaviourEventNames
        = this.serializedObject.FindProperty(nameof(PlayerTriggerVolume.exitBehaviorEventNames));

      this._acceptedPlayerTypes = this.serializedObject.FindProperty(nameof(PlayerTriggerVolume.acceptedPlayerTypes));

      this._enterBehaviorList = new MultiPropertyList(
        new GUIContent("Enter Events"),
        this.serializedObject,
        "Behaviour",
        this._enterBehaviours,
        new[] { "Event" },
        new[] { this._enterBehaviourEventNames });

      this._exitBehaviorList = new MultiPropertyList(
        new GUIContent("Exit Events"),
        this.serializedObject,
        "Behaviour",
        this._exitBehaviours,
        new[] { "Event" },
        new[] { this._exitBehaviourEventNames });
    }

    protected override void RenderInspectorGUI() {
      // ReorderableLists look weird when placed at the very start of the inspector
      EditorGUILayout.Space(20);
      this._enterBehaviorList.DoLayout();
      this._exitBehaviorList.DoLayout();
      EditorGUILayout.Space(20);
      
      EditorGUILayout.LabelField("Advanced Settings", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._acceptedPlayerTypes);
    }
  }
}
