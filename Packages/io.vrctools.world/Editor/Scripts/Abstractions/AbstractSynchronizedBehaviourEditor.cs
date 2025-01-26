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
  /// <summary>
  ///   Provides a specialized extension to the custom Udon editor which handles the fields and functionality provide
  ///   within <see cref="AbstractSynchronizedBehaviour" />.
  /// </summary>
  public abstract class AbstractSynchronizedBehaviourEditor : AbstractCustomUdonEditor {
    private bool _advancedFoldout;
    private SerializedProperty _synchronizationBackoff;
    private SerializedProperty _synchronizationDebugging;

    protected virtual void OnEnable() {
      this._synchronizationBackoff
        = this.serializedObject.FindProperty(nameof(AbstractSynchronizedBehaviour.synchronizationBackoff));
      this._synchronizationDebugging
        = this.serializedObject.FindProperty(nameof(AbstractSynchronizedBehaviour.synchronizationDebugging));
    }

    protected override void RenderInspectorHeader() {
      base.RenderInspectorHeader();

      var targetObject = this.serializedObject.targetObject;
      if (targetObject is Component component)
        CustomEditorUtility.DrawManualSynchronizationWarning(component.gameObject);
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.LabelField("Networking", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._synchronizationBackoff, new GUIContent("Synchronization Backoff"));

      if (this._synchronizationBackoff.floatValue <= 0)
        EditorGUILayout.HelpBox("Backoff has been set to an invalid value\nScript will default to 1 second at runtime",
          MessageType.Error);

      this._advancedFoldout = EditorGUILayout.Foldout(this._advancedFoldout, "Advanced Settings");
      if (this._advancedFoldout) {
        EditorGUI.indentLevel++;
        {
          EditorGUILayout.PropertyField(this._synchronizationDebugging, new GUIContent("Enable Debug Logging"));
        }
        EditorGUI.indentLevel--;
      }
    }
  }
}
