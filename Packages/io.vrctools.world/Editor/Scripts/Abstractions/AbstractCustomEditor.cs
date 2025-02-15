﻿using VRCTools.World.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace VRCTools.World.Editor.Abstractions {
  /// <summary>
  /// Provides a base implementation for custom editors within the assembly.
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
      this._RenderSupporterLinks();
    }

    private void _RenderHelpText() {
      var helpText = this.HelpText;
      if (string.IsNullOrEmpty(helpText)) return;

      EditorGUILayout.Space(20);
      this._helpFoldout = EditorGUILayout.Foldout(this._helpFoldout, "Help");
      if (!this._helpFoldout) return;

      EditorGUILayout.HelpBox(helpText, MessageType.None);
    }

    private void _RenderSupporterLinks() {
      GUILayout.Space(20);
      CustomEditorUtility.DrawLine(Color.gray);

      EditorGUILayout.BeginHorizontal();
      {
        GUILayout.Label($"{CustomEditorUtility.PackageName} v{CustomEditorUtility.PackageVersion}");

        if (EditorGUILayout.LinkButton("Discord")) Application.OpenURL("https://discord.gg/jm6dZ7VHbw");

        if (EditorGUILayout.LinkButton("Patreon")) Application.OpenURL("https://patreon.com/dotStart");
      }
      EditorGUILayout.EndHorizontal();
    }

    protected virtual void OnPropertiesChanged() { this.serializedObject.ApplyModifiedProperties(); }
  }
}
