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
using VRCTools.World.Utils;

namespace VRCTools.World.LocalValues.UI {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [RequireComponent(typeof(Slider))]
  [AddComponentMenu("Local Values/UI/Local Int Slider")]
  public class LocalIntSlider : UdonSharpBehaviour {
    public LocalInt localValue;

    public bool enableMapping;
    public int mappingLowerBound;
    public int mappingUpperBound = 100;
    public int multiplier = 1;

    private Slider _slider;
    private bool _updating;

    private bool RequiresRounding => !this._slider.wholeNumbers;
    private bool AppliesMapping => this.enableMapping && this.localValue.restrictValue;

    private void Start() {
      if (!Utilities.IsValid(this.localValue)) {
        Debug.LogError("[Local Int Slider] Local value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._slider = this.GetComponent<Slider>();

      if (!this._slider.wholeNumbers)
        Debug.LogWarning(
          "[Local Int Slider] Slider has not been configured to use whole numbers - Values will be rounded to the closest whole number",
          this);

      this.localValue._RegisterHandler(LocalInt.EVENT_STATE_UPDATED, this, nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (!Utilities.IsValid(this.localValue)) return;

      this.localValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      this._updating = true;
      {
        var value = this.localValue.State;
        if (this.AppliesMapping)
          value = ValueUtility.Remap(value, this.localValue.restrictedMinimumValue,
            this.localValue.restrictedMaximumValue, this.mappingLowerBound, this.mappingUpperBound);

        value *= this.multiplier;

        this._slider.value = value;
      }
      this._updating = false;
    }

    public void _OnValueChanged() {
      if (this._updating) return;

      int value;
      if (this.RequiresRounding)
        value = Mathf.RoundToInt(this._slider.value);
      else
        value = (int)this._slider.value;

      value /= this.multiplier;
      if (this.AppliesMapping)
        value = ValueUtility.Remap(value, this.mappingLowerBound, this.mappingUpperBound,
          this.localValue.restrictedMinimumValue,
          this.localValue.restrictedMaximumValue);

      this.localValue.State = value;
    }
  }
}
