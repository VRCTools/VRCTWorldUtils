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
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Player;

namespace VRCTools.World.Editor.Player {
  [CustomEditor(typeof(PlayerInteractTrigger))]
  public class PlayerInteractTriggerEditor : AbstractCustomUdonEditor {
    private SerializedProperty _behaviours;
    private MultiPropertyList _behavioursList;
    private SerializedProperty _eventNames;

    private void OnEnable() {
      this._behaviours = this.serializedObject.FindProperty(nameof(PlayerInteractTrigger.behaviours));
      this._eventNames = this.serializedObject.FindProperty(nameof(PlayerInteractTrigger.eventNames));

      this._behavioursList = new MultiPropertyList(
        new GUIContent("Events"),
        this.serializedObject,
        "Behaviour",
        this._behaviours,
        new[] { "Event" },
        new[] { this._eventNames });
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.Space(20);
      this._behavioursList.DoLayout();
    }
  }
}
