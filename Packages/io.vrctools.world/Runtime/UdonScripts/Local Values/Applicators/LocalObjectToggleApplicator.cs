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

namespace VRCTools.World.LocalValues.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Applicators/Object Toggle Applicator")]
  public class LocalObjectToggleApplicator : UdonSharpBehaviour {
    public LocalBoolean localValue;

    public bool invert;

    public GameObject[] targets;

    private void Start() {
      if (!Utilities.IsValid(this.localValue)) {
        Debug.LogError("[Object Toggle Applicator] Local value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this.localValue._RegisterHandler(LocalBoolean.EVENT_STATE_UPDATED, this, nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    public void _OnStateUpdated() {
      var active = this.localValue.State ^ this.invert;

      foreach (var target in this.targets) {
        if (!Utilities.IsValid(target)) {
          continue;
        }

        target.SetActive(active);
      }
    }
  }
}
