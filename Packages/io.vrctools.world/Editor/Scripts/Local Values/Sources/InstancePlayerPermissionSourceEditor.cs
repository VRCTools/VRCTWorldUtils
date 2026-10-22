using UnityEditor;
using VRCTools.World.LocalValues.Sources;

namespace VRCTools.World.Editor.LocalValues.Sources
{
  [CustomEditor(typeof(InstancePlayerPermissionSource))]
  public class InstancePlayerPermissionSourceEditor : AbstractLocalSourceEditor
  {
    private SerializedProperty _permissionType;

    protected override void OnEnable()
    {
      base.OnEnable();

      this._permissionType = this.serializedObject.FindProperty(nameof(InstancePlayerPermissionSource.permissionType));
    }

    protected override void RenderInspectorGUI()
    {
      base.RenderInspectorGUI();
      EditorGUILayout.Space(20);

      EditorGUILayout.PropertyField(this._permissionType);
    }
  }
}
