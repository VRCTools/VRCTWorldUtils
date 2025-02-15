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
using VRCTools.World.Values.UI;

namespace VRCTools.World.Editor.Values.UI {
  [CustomEditor(typeof(ValueUrlLabel))]
  public class ValueUrlLabelEditor : AbstractCustomUdonEditor {
    private SerializedProperty _source;
    private SerializedProperty _localValue;
    private SerializedProperty _synchronizedValue;

    protected override string HelpText =>
      "Displays the current value of a local url using a given string template.\n\n" +
      "This script is expected to be combined with a LocalUrl component and will match its current state.";

    private void OnEnable() {
      this._source = this.serializedObject.FindProperty(nameof(ValueUrlLabel.source));
      this._localValue = this.serializedObject.FindProperty(nameof(ValueUrlLabel.localValue));
      this._synchronizedValue = this.serializedObject.FindProperty(nameof(ValueUrlLabel.synchronizedValue));
    }

    protected override void RenderInspectorGUI() {
      ValueEditorUtility.DrawSourceProperty(new GUIContent("Source"), this._source, this._localValue,
        this._synchronizedValue, "Invalid or unspecified local value reference.\n\n" +
                                 "This component will disable itself upon startup.");
    }
  }
}
