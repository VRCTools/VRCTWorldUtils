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
using UnityEngine.Rendering;
using VRCTools.World.Editor.Utils;

namespace VRCTools.World.Editor.Abstractions {
  /// <summary>
  ///   Provides a base implementation for custom editors within the assembly.
  /// </summary>
  public abstract class AbstractCustomEditor : UnityEditor.Editor {
    private bool _helpFoldout;

    /// <summary>
    ///   Defines a help text which will be shown at the very end of the inspector within a special foldout.
    /// </summary>
    protected virtual string HelpText => string.Empty;

    public override void OnInspectorGUI() {
      this.serializedObject.Update();

      EditorGUI.BeginChangeCheck();
      {
        this.RenderInspectorHeader();
        this.RenderInspectorGUI();
        this.RenderInspectorFooter();
      }
      if (EditorGUI.EndChangeCheck()) this.OnPropertiesChanged();
    }

    protected virtual void RenderInspectorHeader() { }

    protected abstract void RenderInspectorGUI();

    protected virtual void RenderInspectorFooter() {
      this._RenderHelpText();
      CustomEditorUtility.DrawSupporterLinks();
    }

    private void _RenderHelpText() {
      var helpText = this.HelpText;
      if (string.IsNullOrEmpty(helpText)) return;

      EditorGUILayout.Space(20);
      this._helpFoldout = EditorGUILayout.Foldout(this._helpFoldout, "Help");
      if (!this._helpFoldout) return;

      EditorGUILayout.HelpBox(helpText, MessageType.None);
    }

    protected virtual void OnPropertiesChanged() { this.serializedObject.ApplyModifiedProperties(); }
  }
}
