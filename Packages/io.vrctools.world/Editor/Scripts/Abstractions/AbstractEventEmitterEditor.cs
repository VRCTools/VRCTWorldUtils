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
using VRCTools.Event;

namespace VRCTools.World.Editor.Abstractions
{
  public abstract class AbstractEventEmitterEditor : AbstractCustomUdonEditor
  {
    private bool _advancedFoldout;
    private SerializedProperty _eventEmitterDebugging;

    [MustCallBase]
    protected virtual void OnEnable()
    {
      this._eventEmitterDebugging =
        this.serializedObject.FindProperty(nameof(AbstractEventEmitter.eventEmitterDebugging));
    }

    protected override void RenderInspectorGUI()
    {
      this.RenderAdvancedFoldoutGUI();
    }

    protected virtual void RenderAdvancedFoldoutGUI()
    {
      this._advancedFoldout = EditorGUILayout.Foldout(this._advancedFoldout, "Advanced Settings");
      if (this._advancedFoldout)
      {
        EditorGUI.indentLevel++;
        {
          this.RenderAdvancedInspectorGUI();
        }
        EditorGUI.indentLevel--;
      }
    }

    protected virtual void RenderAdvancedInspectorGUI()
    {
      EditorGUILayout.PropertyField(this._eventEmitterDebugging, new GUIContent("Enable Event Logging"));
    }
  }
}
