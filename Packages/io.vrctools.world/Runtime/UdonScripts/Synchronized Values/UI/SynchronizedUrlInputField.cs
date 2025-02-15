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
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace VRCTools.World.SynchronizedValues.UI {
  [RequireComponent(typeof(VRCUrlInputField))]
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/UI/Synchronized URL Input Field")]
  public class SynchronizedUrlInputField : UdonSharpBehaviour {
    public SynchronizedUrl synchronizedValue;

    // TODO: Support filtering

    private VRCUrlInputField _inputField;
    private bool _updating;

    private void Start() {
      if (!Utilities.IsValid(this.synchronizedValue)) {
        Debug.LogError("[Synchronized URL Input Field] Synchronized value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._inputField = this.GetComponent<VRCUrlInputField>();

      this.synchronizedValue._RegisterHandler(SynchronizedUrl.EVENT_STATE_UPDATED, this, nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (!Utilities.IsValid(this.synchronizedValue)) return;

      this.synchronizedValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      this._updating = true;
      {
        this._inputField.SetUrl(this.synchronizedValue.State);
      }
      this._updating = false;
    }

    public void _OnValueChanged() {
      if (this._updating) return;

      this.synchronizedValue.State = this._inputField.GetUrl();
    }
  }
}
