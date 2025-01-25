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
using UnityEngine.UI;
using VRC.SDKBase;

namespace VRCTools.World.SynchronizedValues.UI {
  [RequireComponent(typeof(Toggle))]
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/UI/Synchronized Toggle")]
  public class SynchronizedToggle : UdonSharpBehaviour {
    public SynchronizedBoolean synchronizedValue;

    public bool invert;

    private Toggle _toggle;
    private bool _updating;

    private void Start() {
      if (!Utilities.IsValid(this.synchronizedValue)) {
        Debug.LogError("[Synchronized Toggle] Synchronized value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._toggle = this.GetComponent<Toggle>();

      this.synchronizedValue._RegisterHandler(SynchronizedBoolean.EVENT_STATE_UPDATED, this,
        nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (!Utilities.IsValid(this.synchronizedValue)) return;

      this.synchronizedValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      this._updating = true;
      {
        this._toggle.isOn = this.synchronizedValue.State ^ this.invert;
      }
      this._updating = false;
    }

    public void _OnValueChanged() {
      if (this._updating) return;

      this.synchronizedValue.State = this._toggle.isOn ^ this.invert;
    }
  }
}
