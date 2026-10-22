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

using UnityEditor;
using VRCTools.World.Editor.Values.Sources;
using VRCTools.World.Values;
using VRCTools.World.Values.Sources;

namespace VRCTools.World.Editor.Values.Sources
{
  [CustomEditor(typeof(PlayerCountSource))]
  public class PlayerCountSourceEditor : AbstractSourceEditor
  {
    protected override string HelpText => "Writes the current player count to a given output value.";

    private SerializedProperty _mode;

    protected override void OnEnable()
    {
      base.OnEnable();

      this._mode = this.serializedObject.FindProperty(nameof(PlayerCountSource.mode));
    }

    protected override void RenderInspectorGUI()
    {
      base.RenderInspectorGUI();
      EditorGUILayout.Space(20);

      EditorGUILayout.PropertyField(this._mode);
      var selectedTarget = (ValueType)this._output.enumValueIndex;
      var selectedMode = (PlayerCountMode)this._mode.enumValueIndex;

      if (selectedMode != PlayerCountMode.Current && selectedTarget != ValueType.SYNCHRONIZED)
      {
        EditorGUILayout.Space(10);
        EditorGUILayout.HelpBox(
          "Target value is not synchronized - Each player will compute its own version of the value so they may not agree. Use a synchronized output value if this is an issue.",
          MessageType.Warning);
      }
    }
  }
}
