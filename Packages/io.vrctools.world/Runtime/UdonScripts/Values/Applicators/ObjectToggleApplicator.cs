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

namespace VRCTools.World.Values.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Applicators/Object Toggle Applicator")]
  public class ObjectToggleApplicator : UdonSharpBehaviour {
    public ValueType source;
    public LocalBoolean localValue;
    public SynchronizedBoolean synchronizedValue;

    public bool invert;

    public GameObject[] targets;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localValue, this.synchronizedValue)) {
        Debug.LogError("[Object Toggle Applicator] Source value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      ValueUtility.RegisterUpdateHandler(this.source, this.localValue, this.synchronizedValue, this,
        nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    public void _OnStateUpdated() {
      var active = ValueUtility.GetValue(this.source, this.localValue, this.synchronizedValue) ^ this.invert;

      foreach (var obj in this.targets) {
        if (!Utilities.IsValid(obj)) continue;

        obj.SetActive(active);
      }
    }
  }
}
