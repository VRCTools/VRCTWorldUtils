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
using VRCTools.World.Editor.LocalValues;
using VRCTools.World.PersistedValues;

namespace VRCTools.World.Editor.PersistedValues
{
  using Value = PersistedColor;

  [CustomEditor(typeof(Value))]
  public class PersistedColorEditor : LocalColorEditor
  {
    private SerializedProperty _keyProperty;

    protected override string HelpText =>
      "Provides a persisted color value with event capabilities.\n\n" +
      "You may use this component to create material properties which may be changed on the fly within an instance " +
      "of this world.\n\n" +
      "Please note that this script is useless on its own. You will need to combine it with either custom " +
      "scripts or one of the scripts provided by this package: \n\n" +
      " - For material properties use LocalMaterialPropertyApplicator\n" +
      "Additional compatible scripts may be available within this package.";

    protected override void OnEnable()
    {
      base.OnEnable();

      this._keyProperty = this.serializedObject.FindProperty(nameof(Value.key));
    }

    protected override void RenderInspectorGUI()
    {
      EditorGUILayout.LabelField("Persistence", EditorStyles.boldLabel);
      EditorGUILayout.PropertyField(this._keyProperty);
      EditorGUILayout.Space(20);

      base.RenderInspectorGUI();
    }
  }
}
