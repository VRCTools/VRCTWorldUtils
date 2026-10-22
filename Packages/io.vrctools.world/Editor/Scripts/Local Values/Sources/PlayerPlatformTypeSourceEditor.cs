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
using VRCTools.World.LocalValues.Sources;

namespace VRCTools.World.Editor.LocalValues.Sources
{
  [CustomEditor(typeof(PlayerPlatformTypeSource))]
  public class PlayerPlatformTypeSourceEditor : AbstractLocalSourceEditor
  {
    private SerializedProperty _platformType;

    protected override void OnEnable()
    {
      base.OnEnable();

      this._platformType = this.serializedObject.FindProperty(nameof(PlayerPlatformTypeSource.platformType));
    }

    protected override void RenderInspectorGUI()
    {
      base.RenderInspectorGUI();
      EditorGUILayout.Space(20);

      EditorGUILayout.PropertyField(this._platformType);
    }
  }
}
