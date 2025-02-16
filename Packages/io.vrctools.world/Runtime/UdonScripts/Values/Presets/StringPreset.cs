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

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Presets {
  /// <summary>
  ///   Stores a preset which may be applied to a given string value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Values/Presets/String Preset")]
  public class StringPreset : UdonSharpBehaviour {
    public ValueType target;
    public LocalString localTarget;
    public SynchronizedString synchronizedTarget;

    public string preset;

    private void Start() {
      if (!ValueUtility.IsValid(this.target, this.localTarget, this.synchronizedTarget)) {
        Debug.LogError("[String Preset] Value target is invalid - Disabled", this);
        this.enabled = false;
      }
    }

    public void _Apply() { ValueUtility.SetValue(this.target, this.localTarget, this.synchronizedTarget, this.preset); }
  }
}
