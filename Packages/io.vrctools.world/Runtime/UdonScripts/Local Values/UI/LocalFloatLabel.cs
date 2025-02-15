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

using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.Utils;

namespace VRCTools.World.LocalValues.UI {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [RequireComponent(typeof(TextMeshProUGUI))]
  [AddComponentMenu("Local Values/UI/Local Float Label")]
  public class LocalFloatLabel : UdonSharpBehaviour {
    public LocalFloat localValue;

    public bool enableMapping;
    public int mappingLowerBound;
    public int mappingUpperBound = 100;
    public float multiplier = 100;
    private string _template;

    private TextMeshProUGUI _text;

    private bool AppliesMapping => this.enableMapping && this.localValue.restrictValue;

    private void Start() {
      if (!Utilities.IsValid(this.localValue)) {
        Debug.LogError("[Local Float Label] Local value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._text = this.GetComponent<TextMeshProUGUI>();
      this._template = this._text.text;

      this.localValue._RegisterHandler(LocalFloat.EVENT_STATE_UPDATED, this,
        nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (!Utilities.IsValid(this.localValue)) return;

      this.localValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      var value = this.localValue.State;
      if (this.AppliesMapping)
        value = ValueUtility.Remap(value, this.localValue.restrictedMinimumValue,
          this.localValue.restrictedMaximumValue, this.mappingLowerBound, this.mappingUpperBound);

      value *= this.multiplier;

      this._text.text = string.Format(this._template, value);
    }
  }
}
