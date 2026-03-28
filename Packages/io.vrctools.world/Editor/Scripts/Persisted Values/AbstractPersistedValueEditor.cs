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
using VRCTools.World.Editor.LocalValues;

namespace VRCTools.World.Editor.PersistedValues {
  public abstract class AbstractPersistedValueEditor : AbstractLocalValueEditor {
    protected virtual string _keyPropertyName => "key";

    private SerializedProperty _keyProperty;

    protected override void OnEnable() {
      base.OnEnable();

      this._keyProperty = this.serializedObject.FindProperty(this._keyPropertyName);
    }

    protected override void RenderInspectorGUI() {
      this._RenderPersistenceProperties();
      EditorGUILayout.Space(20);

      base.RenderInspectorGUI();
    }

    protected virtual void _RenderPersistenceProperties() {
      EditorGUILayout.LabelField("Persistence", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._keyProperty);
    }
  }
}
