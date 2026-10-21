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
using VRCTools.Common.Attributes;
using VRCTools.Event;
using VRCTools.World.Editor.Utils;

namespace VRCTools.World.Editor.Abstractions
{
  /// <summary>
  ///   Provides a specialized extension to the custom Udon editor which handles the fields and functionality provide
  ///   within <see cref="AbstractSynchronizedEventEmitter" />.
  /// </summary>
  public abstract class AbstractSynchronizedEventEmitterEditor : AbstractEventEmitterEditor
  {
    private SerializedProperty _synchronizationBackoff;
    private SerializedProperty _synchronizationDebugging;

    [MustCallBase]
    protected override void OnEnable()
    {
      base.OnEnable();

      this._synchronizationBackoff
        = this.serializedObject.FindProperty(nameof(AbstractSynchronizedEventEmitter.synchronizationBackoff));
      this._synchronizationDebugging
        = this.serializedObject.FindProperty(nameof(AbstractSynchronizedEventEmitter.synchronizationDebugging));
    }

    protected override void RenderInspectorHeader()
    {
      base.RenderInspectorHeader();

      var targetObject = this.serializedObject.targetObject;
      if (targetObject is Component component)
        CustomEditorUtility.DrawManualSynchronizationWarning(component.gameObject);
    }

    protected override void RenderInspectorGUI()
    {
      EditorGUILayout.LabelField("Networking", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._synchronizationBackoff, new GUIContent("Synchronization Backoff"));

      base.RenderInspectorGUI();
    }

    protected override void RenderAdvancedInspectorGUI()
    {
      EditorGUILayout.PropertyField(this._synchronizationDebugging, new GUIContent("Enable Synchronization Logging"));
      base.RenderAdvancedInspectorGUI();
    }
  }
}
