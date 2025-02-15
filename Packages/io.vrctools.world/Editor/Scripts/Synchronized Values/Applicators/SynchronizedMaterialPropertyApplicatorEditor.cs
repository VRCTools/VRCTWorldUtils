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
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.SynchronizedValues.Applicators;

namespace VRCTools.World.Editor.SynchronizedValues.Applicators {
  using Applicator = SynchronizedMaterialPropertyApplicator;

  [CustomEditor(typeof(Applicator))]
  public class SynchronizedMaterialPropertyApplicatorEditor : AbstractCustomUdonEditor {
    private SerializedProperty _material;

    private MultiPropertyList _synchronizedBooleanList;
    private SerializedProperty _synchronizedBooleanParameters;

    private SerializedProperty _synchronizedBooleans;
    private MultiPropertyList _synchronizedColorList;
    private SerializedProperty _synchronizedColorParameters;

    private SerializedProperty _synchronizedColors;
    private MultiPropertyList _synchronizedFloatList;
    private SerializedProperty _synchronizedFloatParameters;

    private SerializedProperty _synchronizedFloats;
    private MultiPropertyList _synchronizedIntList;
    private SerializedProperty _synchronizedIntParameters;

    private SerializedProperty _synchronizedInts;
    private MultiPropertyList _synchronizedVectorList;
    private SerializedProperty _synchronizedVectorParameters;

    private SerializedProperty _synchronizedVectors;
    private SerializedProperty _targetRenderer;
    private SerializedProperty _useMaterialBlock;

    protected override string HelpText =>
      "Applies material properties to a material or renderer based on one or more synchronized values.\n\n" +
      "This script is expected to be combined with one or more synchronized value components:\n" +
      " - SynchronizedBoolean\n" +
      " - SynchronizedColor\n" +
      " - SynchronizedFloat\n" +
      " - SynchronizedInt\n" +
      " - SynchronizedVector3";

    private void OnEnable() {
      this._material = this.serializedObject.FindProperty(nameof(Applicator.material));
      this._useMaterialBlock = this.serializedObject.FindProperty(nameof(Applicator.useMaterialBlock));
      this._targetRenderer = this.serializedObject.FindProperty(nameof(Applicator.targetRenderer));

      this._synchronizedBooleans = this.serializedObject.FindProperty(nameof(Applicator.synchronizedBooleans));
      this._synchronizedBooleanParameters
        = this.serializedObject.FindProperty(nameof(Applicator.synchronizedBooleanParameters));

      this._synchronizedColors = this.serializedObject.FindProperty(nameof(Applicator.synchronizedColors));
      this._synchronizedColorParameters
        = this.serializedObject.FindProperty(nameof(Applicator.synchronizedColorParameters));

      this._synchronizedFloats = this.serializedObject.FindProperty(nameof(Applicator.synchronizedFloats));
      this._synchronizedFloatParameters
        = this.serializedObject.FindProperty(nameof(Applicator.synchronizedFloatParameters));

      this._synchronizedInts = this.serializedObject.FindProperty(nameof(Applicator.synchronizedInts));
      this._synchronizedIntParameters
        = this.serializedObject.FindProperty(nameof(Applicator.synchronizedIntParameters));

      this._synchronizedVectors = this.serializedObject.FindProperty(nameof(Applicator.synchronizedVectors));
      this._synchronizedVectorParameters
        = this.serializedObject.FindProperty(nameof(Applicator.synchronizedVectorParameters));

      this._synchronizedBooleanList = new MultiPropertyList(
        new GUIContent("Boolean Properties"),
        this.serializedObject,
        "Synchronized Value",
        this._synchronizedBooleans,
        new[] { "Parameter Name" },
        new[] { this._synchronizedBooleanParameters });

      this._synchronizedColorList = new MultiPropertyList(
        new GUIContent("Color Properties"),
        this.serializedObject,
        "Synchronized Value",
        this._synchronizedColors,
        new[] { "Parameter Name" },
        new[] { this._synchronizedColorParameters });

      this._synchronizedFloatList = new MultiPropertyList(
        new GUIContent("Float Properties"),
        this.serializedObject,
        "Synchronized Value",
        this._synchronizedFloats,
        new[] { "Parameter Name" },
        new[] { this._synchronizedFloatParameters });

      this._synchronizedIntList = new MultiPropertyList(
        new GUIContent("Int Properties"),
        this.serializedObject,
        "Synchronized Value",
        this._synchronizedInts,
        new[] { "Parameter Name" },
        new[] { this._synchronizedIntParameters });

      this._synchronizedVectorList = new MultiPropertyList(
        new GUIContent("Vector Properties"),
        this.serializedObject,
        "Synchronized Value",
        this._synchronizedVectors,
        new[] { "Parameter Name" },
        new[] { this._synchronizedVectorParameters });
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._useMaterialBlock);
      if (this._useMaterialBlock.boolValue) {
        EditorGUILayout.PropertyField(this._targetRenderer);
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
          "Important: Material Property Blocks are only available for shaders which have been explicitly " +
          "designed to support instancing. Additionally, \"Enable GPU instancing\" must be selected within the " +
          "material. If your changes are not taking effect, you may need to apply properties directly to the " +
          "Material or switch to a different shader.",
          MessageType.Info
        );
      } else {
        EditorGUILayout.PropertyField(this._material);
      }

      EditorGUILayout.Space(20);

      this._synchronizedBooleanList.DoLayout();
      this._synchronizedColorList.DoLayout();
      this._synchronizedFloatList.DoLayout();
      this._synchronizedIntList.DoLayout();
      this._synchronizedVectorList.DoLayout();
    }
  }
}
