using System;
using UnityEditor;
using VRC.SDKBase;
using VRCTools.Common.Attributes;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Utils;

namespace VRCTools.World.Editor.LocalValues.Sources
{
  public abstract class AbstractLocalSourceEditor : AbstractCustomUdonEditor
  {
    protected SerializedProperty _localOutput;

    [MustCallBase]
    protected virtual void OnEnable()
    {
      this._localOutput = this.serializedObject.FindProperty("localOutput");
    }

    protected override void RenderInspectorGUI()
    {
      EditorGUILayout.PropertyField(this._localOutput);
      if (!Utilities.IsValid(this._localOutput.objectReferenceValue))
      {
        EditorGUILayout.HelpBox("Output is invalid - Source will not run", MessageType.Error);
      }
    }
  }
}
