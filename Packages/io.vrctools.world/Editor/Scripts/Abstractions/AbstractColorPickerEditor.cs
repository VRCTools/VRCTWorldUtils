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
using VRCTools.World.Abstractions;

namespace VRCTools.World.Editor.Abstractions {
  public abstract class AbstractColorPickerEditor : AbstractCustomUdonEditor {
    private SerializedProperty _crosshair;
    private SerializedProperty _cursor;

    protected virtual void OnEnable() {
      this._cursor = this.serializedObject.FindProperty(nameof(AbstractColorPicker.cursor));
      this._crosshair = this.serializedObject.FindProperty(nameof(AbstractColorPicker.crosshair));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.LabelField("UI Elements", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._cursor);
      EditorGUILayout.PropertyField(this._crosshair);
    }
  }
}
