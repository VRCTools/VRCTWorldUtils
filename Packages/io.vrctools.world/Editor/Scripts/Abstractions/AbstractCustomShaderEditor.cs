using UnityEditor;
using UnityEngine.Rendering;
using VRCTools.World.Editor.Utils;

namespace VRCTools.World.Editor.Abstractions {
  public abstract class AbstractCustomShaderEditor : ShaderGUI {
    /// <summary>
    ///   Defines a help text which will be shown at the very end of the inspector within a special foldout.
    /// </summary>
    protected virtual string HelpText => string.Empty;

    private bool _helpFoldout;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties) {
      materialEditor.SetDefaultGUIWidths();

      this.RenderInspectorGUI(materialEditor, properties);
      this.RenderInspectorFooter(materialEditor, properties);
    }

    protected abstract void RenderInspectorGUI(MaterialEditor me, MaterialProperty[] properties);

    protected virtual void RenderInspectorFooter(MaterialEditor me, MaterialProperty[] properties) {
      EditorGUILayout.Space(20);
      EditorGUILayout.LabelField("Shader Settings", EditorStyles.boldLabel);
      this._RenderGenericProperties(me, properties);

      this._RenderHelpText();
      CustomEditorUtility.DrawSupporterLinks();
    }

    protected virtual void _RenderGenericProperties(MaterialEditor me, MaterialProperty[] properties) {
      if (SupportedRenderingFeatures.active.editableMaterialRenderQueue)
        me.RenderQueueField();

      me.EnableInstancingField();
      me.DoubleSidedGIField();
    }

    protected virtual void _RenderHelpText() {
      var helpText = this.HelpText;
      if (string.IsNullOrEmpty(helpText)) return;

      EditorGUILayout.Space(20);
      this._helpFoldout = EditorGUILayout.Foldout(this._helpFoldout, "Help");
      if (!this._helpFoldout) return;

      EditorGUILayout.HelpBox(helpText, MessageType.None);
    }
  }
}
