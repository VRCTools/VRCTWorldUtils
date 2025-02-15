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
using VRCTools.World.LocalValues.Applicators;

namespace VRCTools.World.Editor.LocalValues.Applicators {
  using Applicator = LocalMaterialPropertyApplicator;

  [CustomEditor(typeof(Applicator))]
  public class LocalMaterialPropertyApplicatorEditor : AbstractCustomUdonEditor {
    private MultiPropertyList _localBooleanList;
    private SerializedProperty _localBooleanParameters;
    private SerializedProperty _localBooleans;

    private MultiPropertyList _localColorList;
    private SerializedProperty _localColorParameters;
    private SerializedProperty _localColors;

    private MultiPropertyList _localFloatList;
    private SerializedProperty _localFloatParameters;
    private SerializedProperty _localFloats;

    private MultiPropertyList _localIntList;
    private SerializedProperty _localIntParameters;
    private SerializedProperty _localInts;

    private MultiPropertyList _localTextureList;
    private SerializedProperty _localTextureParameters;
    private SerializedProperty _localTextures;

    private MultiPropertyList _localVectorList;
    private SerializedProperty _localVectorParameters;
    private SerializedProperty _localVectors;

    private SerializedProperty _material;
    private SerializedProperty _targetRenderer;
    private SerializedProperty _useMaterialBlock;

    protected override string HelpText =>
      "Applies material properties to a material or renderer based on one or more local values.\n\n" +
      "This script is expected to be combined with one or more local value components:\n" +
      " - LocalBoolean\n" +
      " - LocalColor\n" +
      " - LocalFloat\n" +
      " - LocalInt\n" +
      " - LocalVector3";

    private void OnEnable() {
      this._material = this.serializedObject.FindProperty(nameof(Applicator.material));
      this._useMaterialBlock = this.serializedObject.FindProperty(nameof(Applicator.usePropertyBlock));
      this._targetRenderer = this.serializedObject.FindProperty(nameof(Applicator.targetRenderer));

      this._localBooleans = this.serializedObject.FindProperty(nameof(Applicator.localBooleans));
      this._localBooleanParameters
        = this.serializedObject.FindProperty(nameof(Applicator.localBooleanParameters));

      this._localColors = this.serializedObject.FindProperty(nameof(Applicator.localColors));
      this._localColorParameters
        = this.serializedObject.FindProperty(nameof(Applicator.localColorParameters));

      this._localFloats = this.serializedObject.FindProperty(nameof(Applicator.localFloats));
      this._localFloatParameters
        = this.serializedObject.FindProperty(nameof(Applicator.localFloatParameters));

      this._localInts = this.serializedObject.FindProperty(nameof(Applicator.localInts));
      this._localIntParameters
        = this.serializedObject.FindProperty(nameof(Applicator.localIntParameters));

      this._localTextures = this.serializedObject.FindProperty(nameof(Applicator.localTextures));
      this._localTextureParameters = this.serializedObject.FindProperty(nameof(Applicator.localTextureParameters));

      this._localVectors = this.serializedObject.FindProperty(nameof(Applicator.localVectors));
      this._localVectorParameters
        = this.serializedObject.FindProperty(nameof(Applicator.localVectorParameters));

      this._localBooleanList = new MultiPropertyList(
        new GUIContent("Boolean Properties"),
        this.serializedObject,
        "Local Value",
        this._localBooleans,
        new[] { "Parameter Name" },
        new[] { this._localBooleanParameters });

      this._localColorList = new MultiPropertyList(
        new GUIContent("Color Properties"),
        this.serializedObject,
        "Local Value",
        this._localColors,
        new[] { "Parameter Name" },
        new[] { this._localColorParameters });

      this._localFloatList = new MultiPropertyList(
        new GUIContent("Float Properties"),
        this.serializedObject,
        "Local Value",
        this._localFloats,
        new[] { "Parameter Name" },
        new[] { this._localFloatParameters });

      this._localIntList = new MultiPropertyList(
        new GUIContent("Int Properties"),
        this.serializedObject,
        "Local Value",
        this._localInts,
        new[] { "Parameter Name" },
        new[] { this._localIntParameters });

      this._localTextureList = new MultiPropertyList(
        new GUIContent("Texture Properties"),
        this.serializedObject,
        "Local Value",
        this._localTextures,
        new[] { "Parameter Name" },
        new[] { this._localTextureParameters });

      this._localVectorList = new MultiPropertyList(
        new GUIContent("Vector Properties"),
        this.serializedObject,
        "Local Value",
        this._localVectors,
        new[] { "Parameter Name" },
        new[] { this._localVectorParameters });
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

      this._localBooleanList.DoLayout();
      this._localColorList.DoLayout();
      this._localFloatList.DoLayout();
      this._localIntList.DoLayout();
      this._localTextureList.DoLayout();
      this._localVectorList.DoLayout();
    }
  }
}
