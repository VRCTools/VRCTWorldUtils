// Copyright 2026 .start <https://dotstart.tv>
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

using System;
using UnityEditor;
using UnityEngine;
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.Editor.Utils;
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.Editor.Values.Applicators
{
  [CustomEditor(typeof(PostProcessingVolumeApplicator))]
  public class PostProcessingVolumeApplicatorEditor : AbstractCustomUdonEditor
  {
    private SerializedProperty _volume;

    private SerializedProperty _weight;
    private SerializedProperty _localWeight;
    private SerializedProperty _synchronizedWeight;

    private void OnEnable()
    {
      this._volume = this.serializedObject.FindProperty(nameof(PostProcessingVolumeApplicator.volume));

      this._weight = this.serializedObject.FindProperty(nameof(PostProcessingVolumeApplicator.weight));
      this._localWeight = this.serializedObject.FindProperty(nameof(PostProcessingVolumeApplicator.localWeight));
      this._synchronizedWeight =
        this.serializedObject.FindProperty(nameof(PostProcessingVolumeApplicator.synchronizedWeight));
    }

    protected override void RenderInspectorGUI()
    {
      EditorGUILayout.PropertyField(this._volume);
      EditorGUILayout.Space(20);

      EditorGUILayout.LabelField("Weight", EditorStyles.boldLabel);
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Weight"), this._weight, this._localWeight,
        this._synchronizedWeight, "Weight is unspecified - Component will not run");
    }
  }
}
