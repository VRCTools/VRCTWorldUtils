using System;
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.LocalValues.Applicators;

namespace VRCTools.World.Editor.LocalValues.Applicators {
  using Applicator = LocalAnimatorPropertyApplicator;

  [CustomEditor(typeof(Applicator))]
  public class LocalAnimatorPropertyApplicatorEditor : AbstractCustomUdonEditor {
    private SerializedProperty _target;

    private SerializedProperty _localBooleans;
    private SerializedProperty _localBooleanParameters;

    private SerializedProperty _localFloats;
    private SerializedProperty _localFloatParameters;

    private SerializedProperty _localInts;
    private SerializedProperty _localIntParameters;

    private MultiPropertyList _localBooleanList;
    private MultiPropertyList _localFloatList;
    private MultiPropertyList _localIntList;

    protected override string HelpText => "Applies local values to a given set of animator parameters.";

    private void OnEnable() {
      this._target = this.serializedObject.FindProperty(nameof(Applicator.target));

      this._localBooleans = this.serializedObject.FindProperty(nameof(Applicator.localBooleans));
      this._localBooleanParameters = this.serializedObject.FindProperty(nameof(Applicator.localBooleanParameters));

      this._localFloats = this.serializedObject.FindProperty(nameof(Applicator.localFloats));
      this._localFloatParameters = this.serializedObject.FindProperty(nameof(Applicator.localFloatParameters));

      this._localInts = this.serializedObject.FindProperty(nameof(Applicator.localInts));
      this._localIntParameters = this.serializedObject.FindProperty(nameof(Applicator.localIntParameters));

      this._localBooleanList = new MultiPropertyList(new GUIContent("Boolean Parameters"), this.serializedObject,
        "Value", this._localBooleans, new[] { "Parameter Name" },
        new[] { this._localBooleanParameters });

      this._localFloatList = new MultiPropertyList(new GUIContent("Float Parameters"), this.serializedObject, "Value",
        this._localFloats, new[] { "Parameter Name" }, new[] { this._localFloatParameters });

      this._localIntList = new MultiPropertyList(new GUIContent("Int Parameters"), this.serializedObject, "Value",
        this._localInts, new[] { "Parameter Name" }, new[] { this._localIntParameters });
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._target);
      if (!Utilities.IsValid(this._target.objectReferenceValue)) {
        EditorGUILayout.HelpBox(
          "Target animator has not been set or is invalid - Component will disable itself on startup",
          MessageType.Error);
      }

      EditorGUILayout.Space(20);

      this._localBooleanList.DoLayout();
      this._localFloatList.DoLayout();
      this._localIntList.DoLayout();
    }
  }
}
