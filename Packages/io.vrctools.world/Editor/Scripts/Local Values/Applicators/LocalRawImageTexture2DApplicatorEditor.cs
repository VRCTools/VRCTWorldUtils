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
using VRCTools.World.Editor.Abstractions;
using VRCTools.World.LocalValues.Applicators;

namespace VRCTools.World.Editor.LocalValues.Applicators {
  using Applicator = LocalRawImageTexture2DApplicator;

  [CustomEditor(typeof(Applicator))]
  public class LocalRawImageTexture2DApplicatorEditor : AbstractCustomUdonEditor {
    private SerializedProperty _localValue;

    protected override string HelpText =>
      "Applies a given texture to a RawImage component.\n\n" +
      "This script is expected to be combined with a LocalTexture2D component.";

    private void OnEnable() {
      this._localValue
        = this.serializedObject.FindProperty(nameof(Applicator.localValue));
    }

    protected override void RenderInspectorGUI() {
      EditorGUILayout.PropertyField(this._localValue);
      if (this._localValue.objectReferenceValue == null)
        EditorGUILayout.HelpBox(
          "Invalid or unspecified local value reference.\n\n" +
          "This component will disable itself upon startup.",
          MessageType.Warning);
    }
  }
}
