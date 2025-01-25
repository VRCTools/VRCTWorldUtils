using UdonSharpEditor;

namespace VRCTools.World.Editor.Abstractions {
  /// <summary>
  /// Provides a specialized variation of the abstract custom editor implementation which includes Udon related UI
  /// elements by default.
  /// </summary>
  public abstract class AbstractCustomUdonEditor : AbstractCustomEditor {
    public override void OnInspectorGUI() {
      if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(this.target)) return;

      base.OnInspectorGUI();
    }
  }
}
