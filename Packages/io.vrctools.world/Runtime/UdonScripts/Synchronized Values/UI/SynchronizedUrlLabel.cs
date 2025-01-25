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
  [RequireComponent(typeof(TextMeshProUGUI))]
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/UI/Synchronized URL Label")]
  public class SynchronizedUrlLabel : UdonSharpBehaviour {
    public SynchronizedUrl synchronizedValue;
    private string _template;

    private TextMeshProUGUI _text;

    private void Start() {
      if (!Utilities.IsValid(this.synchronizedValue)) {
        Debug.LogError("[Synchronized URL Label] Synchronized value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._text = this.GetComponent<TextMeshProUGUI>();
      this._template = this._text.text;

      this.synchronizedValue._RegisterHandler(SynchronizedUrl.EVENT_STATE_UPDATED, this, nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (!Utilities.IsValid(this.synchronizedValue)) return;

      this.synchronizedValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() { this._text.text = string.Format(this._template, this.synchronizedValue.State); }
  }
}
