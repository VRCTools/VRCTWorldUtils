﻿using UnityEditor;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.LocalValues.Converters;

namespace VRCTools.World.Editor.SynchronizedValues.Converters {
  using Converter = SynchronizedTexture2DLoaderConverter;

  [CustomEditor(typeof(Converter))]
  public class SynchronizedTexture2DLoaderConverterEditor : AbstractCustomUdonEditor {
    private SerializedProperty _localUrl;
    private SerializedProperty _localTexture2D;

    private SerializedProperty _defaultTexture;
    private SerializedProperty _loadingTexture;
    private SerializedProperty _errorTexture;
    private SerializedProperty _accessDeniedErrorTexture;
    private SerializedProperty _downloadErrorTexture;
    private SerializedProperty _invalidErrorTexture;
    private SerializedProperty _tooManyRequestsErrorTexture;
    private SerializedProperty _invalidUrlErrorTexture;

    private SerializedProperty _retryPeriod;

    private bool _errorFoldout;

    protected override string HelpText =>
      "Retrieves images from a web server based on a synchronized url value.\n\n" +
      "This script is expected to be combined with a SynchronizedUrl and LocalTexture2D component.";

    private void OnEnable() {
      this._localUrl = this.serializedObject.FindProperty(nameof(Converter.synchronizedUrl));
      this._localTexture2D = this.serializedObject.FindProperty(nameof(Converter.localTexture2D));

      this._defaultTexture = this.serializedObject.FindProperty(nameof(Converter.defaultTexture));
      this._loadingTexture = this.serializedObject.FindProperty(nameof(Converter.loadingTexture));
      this._errorTexture = this.serializedObject.FindProperty(nameof(Converter.errorTexture));
      this._accessDeniedErrorTexture = this.serializedObject.FindProperty(nameof(Converter.accessDeniedErrorTexture));
      this._downloadErrorTexture = this.serializedObject.FindProperty(nameof(Converter.downloadErrorTexture));
      this._invalidErrorTexture = this.serializedObject.FindProperty(nameof(Converter.invalidErrorTexture));
      this._tooManyRequestsErrorTexture
        = this.serializedObject.FindProperty(nameof(Converter.tooManyRequestsErrorTexture));
      this._invalidUrlErrorTexture = this.serializedObject.FindProperty(nameof(Converter.invalidUrlErrorTexture));

      this._retryPeriod = this.serializedObject.FindProperty(nameof(Converter.retryPeriod));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._localUrl);
      if (this._localUrl.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified local url reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
      EditorGUILayout.PropertyField(this._localTexture2D);
      if (this._localTexture2D.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified local texture reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Alternative Textures", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._defaultTexture);
      EditorGUILayout.PropertyField(this._loadingTexture);
      EditorGUILayout.PropertyField(this._errorTexture);

      this._errorFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(this._errorFoldout, "Errors");
      if (this._errorFoldout) {
        EditorGUI.indentLevel++;
        {
          EditorGUILayout.PropertyField(this._accessDeniedErrorTexture);
          EditorGUILayout.PropertyField(this._downloadErrorTexture);
          EditorGUILayout.PropertyField(this._invalidErrorTexture);
          EditorGUILayout.PropertyField(this._tooManyRequestsErrorTexture);
          EditorGUILayout.PropertyField(this._invalidUrlErrorTexture);
        }
        EditorGUI.indentLevel--;
      }

      EditorGUILayout.EndFoldoutHeaderGroup();
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Error Handling", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._retryPeriod);
    }
  }
}