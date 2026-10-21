// Copyright 2026 .start <https://dotstart.tv>
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
using VRCTools.Common.Attributes;
using VRCTools.World.Editor.Abstractions;

namespace VRCTools.World.Editor.LocalValues {
  public abstract class AbstractLocalValueEditor : AbstractEventEmitterEditor {
    protected virtual string _defaultValuePropertyName => "defaultValue";

    protected SerializedProperty _defaultValue;

    [MustCallBase]
    protected override void OnEnable() {
      base.OnEnable();
      
      this._defaultValue = this.serializedObject.FindProperty(this._defaultValuePropertyName);
    }

    protected override void RenderInspectorGUI() {
      this.RenderDefaults();
      EditorGUILayout.Space(20);
      
      base.RenderInspectorGUI();
    }

    protected virtual void RenderDefaults() {
      EditorGUILayout.LabelField("Defaults", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._defaultValue);
    }
  }
}
