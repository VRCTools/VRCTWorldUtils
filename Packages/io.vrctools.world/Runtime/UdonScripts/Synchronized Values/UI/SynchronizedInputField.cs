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

using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace VRCTools.World.SynchronizedValues.UI {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [RequireComponent(typeof(TMP_InputField))]
  [AddComponentMenu("Synchronized Values/UI/Synchronized Input Field")]
  public class SynchronizedInputField : UdonSharpBehaviour {
    public SynchronizedString synchronizedValue;

    private TMP_InputField _inputField;
    private bool _updating;

    private void Start() {
      if (!Utilities.IsValid(this.synchronizedValue)) {
        Debug.LogError("[Synchronized Input Field] Synchronized value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._inputField = this.GetComponent<TMP_InputField>();

      this.synchronizedValue._RegisterHandler(SynchronizedString.EVENT_STATE_UPDATED, this,
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
        this._inputField.text = this.synchronizedValue.State;
      }
      this._updating = false;
    }

    public void _OnValueChanged() {
      if (this._updating) return;

      this.synchronizedValue.State = this._inputField.text;
    }
  }
}
