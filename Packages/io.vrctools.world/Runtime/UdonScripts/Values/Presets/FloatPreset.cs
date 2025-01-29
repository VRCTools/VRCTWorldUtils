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

namespace VRCTools.World.Values.Presets {
  /// <summary>
  ///   Stores a preset which may be applied to a given float value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Values/Presets/Float Preset")]
  public class FloatPreset : UdonSharpBehaviour {
    public bool useSynchronizedTarget;
    public LocalFloat localTarget;
    public SynchronizedFloat synchronizedTarget;

    public float preset;

    private void Start() {
      if (this.useSynchronizedTarget) {
        if (Utilities.IsValid(this.synchronizedTarget)) return;

        Debug.LogError("[Float Preset] Synchronized target is invalid - Disabled", this);
        this.enabled = false;
      } else if (!Utilities.IsValid(this.localTarget)) {
        Debug.LogError("[Float Preset] Local target is invalid - Disabled", this);
        this.enabled = false;
      }
    }

    public void _Apply() {
      if (this.useSynchronizedTarget) {
        if (!Utilities.IsValid(this.synchronizedTarget)) {
          Debug.LogError("[Float Preset] Synchronized target is invalid - Ignored", this);
          this.enabled = false;
          return;
        }

        this.synchronizedTarget.State = this.preset;
      } else {
        if (!Utilities.IsValid(this.localTarget)) {
          Debug.LogError("[Float Preset] Local target is invalid - Ignored", this);
          this.enabled = false;
          return;
        }

        this.localTarget.State = this.preset;
      }
    }
  }
}
