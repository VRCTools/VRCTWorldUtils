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
using VRCTools.World.Values.Converters;

namespace Editor.Scripts.Values.Converters {
  [CustomEditor(typeof(ColorComponentConverter))]
  public class ColorComponentConverterEditor : AbstractCustomUdonEditor {
    public SerializedProperty _useSynchronizedTarget;
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
      this._useSynchronizedTarget
        = this.serializedObject.FindProperty(nameof(ColorComponentConverter.useSynchronizedTarget));
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
      EditorGUILayout.PropertyField(this._useSynchronizedTarget);
      EditorGUILayout.PropertyField(
        this._useSynchronizedTarget.boolValue ? this._synchronizedTarget : this._localTarget);

      var useSynchronizedTarget = this._useSynchronizedTarget.boolValue;
      if ((useSynchronizedTarget && !Utilities.IsValid(this._synchronizedTarget.objectReferenceValue)) ||
          (!useSynchronizedTarget && !Utilities.IsValid(this._localTarget.objectReferenceValue))) {
        EditorGUILayout.HelpBox("Target value is invalid or unset - Component will be disabled on start",
          MessageType.Error);
      }

      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Red Channel", EditorStyles.boldLabel);
      _DrawProperty(this._redSource, this._staticRedValue, this._localRedValue, this._synchronizedRedValue);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Green Channel", EditorStyles.boldLabel);
      _DrawProperty(this._greenSource, this._staticGreenValue, this._localGreenValue, this._synchronizedGreenValue);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Blue Channel", EditorStyles.boldLabel);
      _DrawProperty(this._blueSource, this._staticBlueValue, this._localBlueValue, this._synchronizedBlueValue);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Alpha Channel", EditorStyles.boldLabel);
      _DrawProperty(this._alphaSource, this._staticAlphaValue, this._localAlphaValue, this._synchronizedAlphaValue);
    }

    private static void _DrawProperty(
      SerializedProperty source,
      SerializedProperty staticProperty,
      SerializedProperty localProperty,
      SerializedProperty synchronizedProperty) {
      EditorGUILayout.PropertyField(source);
      var s = (ValueSource)source.enumValueIndex;

      SerializedProperty property = null;
      switch (s) {
        case ValueSource.STATIC:
          property = staticProperty;
          break;
        case ValueSource.LOCAL:
          property = localProperty;
          break;
        case ValueSource.SYNCHRONIZED:
          property = synchronizedProperty;
          break;
      }

      EditorGUILayout.PropertyField(property, new GUIContent("Source"));
      if (s != ValueSource.STATIC && !Utilities.IsValid(property.objectReferenceValue)) {
        EditorGUILayout.HelpBox("Source value is invalid or unset - Component will be set to zero", MessageType.Error);
      }
    }
  }
}
