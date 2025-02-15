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
using VRCTools.World.Values;
using VRCTools.World.Values.Converters;

namespace VRCTools.World.Editor.Values.Converters {
  [CustomEditor(typeof(ColorComponentConverter))]
  public class ColorComponentConverterEditor : AbstractCustomUdonEditor {
    public SerializedProperty _target;
    public SerializedProperty _localTarget;
    public SerializedProperty _synchronizedTarget;

    public SerializedProperty _redSource;
    public SerializedProperty _staticRedValue;
    public SerializedProperty _localRedValue;
    public SerializedProperty _synchronizedRedValue;

    public SerializedProperty _greenSource;
    public SerializedProperty _staticGreenValue;
    public SerializedProperty _localGreenValue;
    public SerializedProperty _synchronizedGreenValue;

    public SerializedProperty _blueSource;
    public SerializedProperty _staticBlueValue;
    public SerializedProperty _localBlueValue;
    public SerializedProperty _synchronizedBlueValue;

    public SerializedProperty _alphaSource;
    public SerializedProperty _staticAlphaValue;
    public SerializedProperty _localAlphaValue;
    public SerializedProperty _synchronizedAlphaValue;

    private void OnEnable() {
      this._target = this.serializedObject.FindProperty(nameof(ColorComponentConverter.target));
      this._localTarget = this.serializedObject.FindProperty(nameof(ColorComponentConverter.localTarget));
      this._synchronizedTarget = this.serializedObject.FindProperty(nameof(ColorComponentConverter.synchronizedTarget));

      this._redSource = this.serializedObject.FindProperty(nameof(ColorComponentConverter.redSource));
      this._staticRedValue = this.serializedObject.FindProperty(nameof(ColorComponentConverter.staticRedValue));
      this._localRedValue = this.serializedObject.FindProperty(nameof(ColorComponentConverter.localRedValue));
      this._synchronizedRedValue
        = this.serializedObject.FindProperty(nameof(ColorComponentConverter.synchronizedRedValue));

      this._greenSource = this.serializedObject.FindProperty(nameof(ColorComponentConverter.greenSource));
      this._staticGreenValue = this.serializedObject.FindProperty(nameof(ColorComponentConverter.staticGreenValue));
      this._localGreenValue = this.serializedObject.FindProperty(nameof(ColorComponentConverter.localGreenValue));
      this._synchronizedGreenValue
        = this.serializedObject.FindProperty(nameof(ColorComponentConverter.synchronizedGreenValue));

      this._blueSource = this.serializedObject.FindProperty(nameof(ColorComponentConverter.blueSource));
      this._staticBlueValue = this.serializedObject.FindProperty(nameof(ColorComponentConverter.staticBlueValue));
      this._localBlueValue = this.serializedObject.FindProperty(nameof(ColorComponentConverter.localBlueValue));
      this._synchronizedBlueValue
        = this.serializedObject.FindProperty(nameof(ColorComponentConverter.synchronizedBlueValue));

      this._alphaSource = this.serializedObject.FindProperty(nameof(ColorComponentConverter.alphaSource));
      this._staticAlphaValue = this.serializedObject.FindProperty(nameof(ColorComponentConverter.staticAlphaValue));
      this._localAlphaValue = this.serializedObject.FindProperty(nameof(ColorComponentConverter.localAlphaValue));
      this._synchronizedAlphaValue
        = this.serializedObject.FindProperty(nameof(ColorComponentConverter.synchronizedAlphaValue));
    }

    protected override void RenderInspectorGUI() {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("target"), this._target, this._localTarget,
        this._synchronizedTarget, "Target value is invalid or unset - Component will be disabled on start");

      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Red Channel", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._redSource, this._staticRedValue,
        this._localRedValue, this._synchronizedRedValue,
        "Component value is invalid or unset - It will be set to zero");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Green Channel", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._greenSource, this._staticGreenValue,
        this._localGreenValue, this._synchronizedGreenValue,
        "Component value is invalid or unset - It will be set to zero");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Blue Channel", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._blueSource, this._staticBlueValue,
        this._localBlueValue, this._synchronizedBlueValue,
        "Component value is invalid or unset - It will be set to zero");
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Alpha Channel", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Component"), this._alphaSource, this._staticAlphaValue,
        this._localAlphaValue, this._synchronizedAlphaValue,
        "Component value is invalid or unset - It will be set to zero");
    }
  }
}
