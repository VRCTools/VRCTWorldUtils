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
using VRC.SDKBase;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.SynchronizedValues.Applicators;

namespace VRCTools.World.Editor.SynchronizedValues.Applicators {
  using Applicator = SynchronizedAnimatorPropertyApplicator;

  [CustomEditor(typeof(Applicator))]
  public class SynchronizedAnimatorPropertyApplicatorEditor : AbstractCustomUdonEditor {
    private SerializedProperty _target;

    private SerializedProperty _synchronizedBooleans;
    private SerializedProperty _synchronizedBooleanParameters;

    private SerializedProperty _synchronizedFloats;
    private SerializedProperty _synchronizedFloatParameters;

    private SerializedProperty _synchronizedInts;
    private SerializedProperty _synchronizedIntParameters;

    private MultiPropertyList _synchronizedBooleanList;
    private MultiPropertyList _synchronizedFloatList;
    private MultiPropertyList _synchronizedIntList;

    protected override string HelpText => "Applies synchronized values to a given set of animator parameters.";

    private void OnEnable() {
      this._target = this.serializedObject.FindProperty(nameof(Applicator.target));

      this._synchronizedBooleans = this.serializedObject.FindProperty(nameof(Applicator.synchronizedBooleans));
      this._synchronizedBooleanParameters
        = this.serializedObject.FindProperty(nameof(Applicator.synchronizedBooleanParameters));

      this._synchronizedFloats = this.serializedObject.FindProperty(nameof(Applicator.synchronizedFloats));
      this._synchronizedFloatParameters
        = this.serializedObject.FindProperty(nameof(Applicator.synchronizedFloatParameters));

      this._synchronizedInts = this.serializedObject.FindProperty(nameof(Applicator.synchronizedInts));
      this._synchronizedIntParameters
        = this.serializedObject.FindProperty(nameof(Applicator.synchronizedIntParameters));

      this._synchronizedBooleanList = new MultiPropertyList(new GUIContent("Boolean Parameters"), this.serializedObject,
        "Value", this._synchronizedBooleans, new[] { "Parameter Name" },
        new[] { this._synchronizedBooleanParameters });

      this._synchronizedFloatList = new MultiPropertyList(new GUIContent("Float Parameters"), this.serializedObject,
        "Value",
        this._synchronizedFloats, new[] { "Parameter Name" }, new[] { this._synchronizedFloatParameters });

      this._synchronizedIntList = new MultiPropertyList(new GUIContent("Int Parameters"), this.serializedObject,
        "Value",
        this._synchronizedInts, new[] { "Parameter Name" }, new[] { this._synchronizedIntParameters });
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._target);
      if (!Utilities.IsValid(this._target.objectReferenceValue)) {
        EditorGUILayout.HelpBox(
          "Target animator has not been set or is invalid - Component will disable itself on startup",
          MessageType.Error);
      }

      EditorGUILayout.Space(20);

      this._synchronizedBooleanList.DoLayout();
      this._synchronizedFloatList.DoLayout();
      this._synchronizedIntList.DoLayout();
    }
  }
}
