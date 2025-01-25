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
using VRCTools.World.Utils;
using UdonSharp;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VRC.SDKBase;

namespace VRCTools.World.SynchronizedValues.UI {
  [RequireComponent(typeof(Slider))]
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/UI/Synchronized Slider")]
  public class SynchronizedSlider : UdonSharpBehaviour {
    [FormerlySerializedAs("value")]
    public SynchronizedFloat synchronizedValue;

    public bool enableMapping;
    public int mappingLowerBound;
    public int mappingUpperBound = 100;
    public int multiplier = 1;

    private Slider _slider;
    private bool _updating;

    private bool AppliesMapping => this.enableMapping && this.synchronizedValue.restrictValue;

    private void Start() {
      if (!Utilities.IsValid(this.synchronizedValue)) {
        Debug.LogError("[Synchronized Slider] Synchronized value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._slider = this.GetComponent<Slider>();

      this.synchronizedValue._RegisterHandler(SynchronizedFloat.EVENT_STATE_UPDATED, this,
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
        var value = this.synchronizedValue.State;
        if (this.AppliesMapping)
          value = ValueUtility.Remap(value, this.synchronizedValue.restrictedMinimumValue,
            this.synchronizedValue.restrictedMaximumValue, this.mappingLowerBound, this.mappingUpperBound);

        value *= this.multiplier;

        this._slider.value = value;
      }
      this._updating = false;
    }

    public void _OnValueChanged() {
      if (this._updating) return;

      var value = this._slider.value / this.multiplier;
      if (this.AppliesMapping)
        value = ValueUtility.Remap(value, this.mappingLowerBound, this.mappingUpperBound,
          this.synchronizedValue.restrictedMinimumValue, this.synchronizedValue.restrictedMaximumValue);

      this.synchronizedValue.State = value;
    }
  }
}
