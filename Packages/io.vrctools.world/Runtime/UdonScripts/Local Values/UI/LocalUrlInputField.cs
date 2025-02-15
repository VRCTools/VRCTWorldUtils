﻿// Copyright 2025 .start <https://dotstart.tv>
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

namespace VRCTools.World.LocalValues.UI {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [RequireComponent(typeof(VRCUrlInputField))]
  [AddComponentMenu("Local Values/UI/Local URL Input Field")]
  public class LocalUrlInputField : UdonSharpBehaviour {
    public LocalUrl localValue;

    // TODO: Support filtering

    private VRCUrlInputField _inputField;
    private bool _updating;

    private void Start() {
      if (!Utilities.IsValid(this.localValue)) {
        Debug.LogError("[Local URL Input Field] Local value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._inputField = this.GetComponent<VRCUrlInputField>();

      this.localValue._RegisterHandler(LocalUrl.EVENT_STATE_UPDATED, this, nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (!Utilities.IsValid(this.localValue)) return;

      this.localValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      this._updating = true;
      {
        this._inputField.SetUrl(this.localValue.State);
      }
      this._updating = false;
    }

    public void _OnValueChanged() {
      if (this._updating) return;

      this.localValue.State = this._inputField.GetUrl();
    }
  }
}
