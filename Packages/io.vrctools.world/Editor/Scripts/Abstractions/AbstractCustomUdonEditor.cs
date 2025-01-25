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
using UdonSharpEditor;

namespace VRCTools.World.Editor.Abstractions {
  /// <summary>
  /// Provides a specialized variation of the abstract custom editor implementation which includes Udon related UI
  /// elements by default.
  /// </summary>
  public abstract class AbstractCustomUdonEditor : AbstractCustomEditor {
    public override void OnInspectorGUI() {
      if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(this.target)) return;

      base.OnInspectorGUI();
    }
  }
}
