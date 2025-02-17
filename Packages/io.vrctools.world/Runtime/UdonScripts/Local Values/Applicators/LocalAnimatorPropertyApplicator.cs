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
using VRCTools.World.Utils;

namespace VRCTools.World.LocalValues.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Applicators/Local Animator Property Applicator")]
  public class LocalAnimatorPropertyApplicator : UdonSharpBehaviour {
    public Animator target;

    public LocalBoolean[] localBooleans;
    public string[] localBooleanParameters;

    public LocalFloat[] localFloats;
    public string[] localFloatParameters;

    public LocalInt[] localInts;
    public string[] localIntParameters;

    private int[] _localBooleanParameterIds;
    private int[] _localFloatParameterIds;
    private int[] _localIntParameterIds;

    private void Start() {
      if (!Utilities.IsValid(this.target)) {
        Debug.LogError("Local Animator Property Applicator] Animator is invalid - Disabled");
        this.enabled = false;
      }

      this.localBooleans = MappingUtility.CheckCoherentMapping(this.localBooleans, this.localBooleanParameters, this);
      this.localFloats = MappingUtility.CheckCoherentMapping(this.localFloats, this.localFloatParameters, this);
      this.localInts = MappingUtility.CheckCoherentMapping(this.localInts, this.localIntParameters, this);

      this._localBooleanParameterIds = AnimatorPropertyUtility.ResolveParameterIds(this.localBooleanParameters);
      this._localFloatParameterIds = AnimatorPropertyUtility.ResolveParameterIds(this.localFloatParameters);
      this._localIntParameterIds = AnimatorPropertyUtility.ResolveParameterIds(this.localIntParameters);

      foreach (var value in this.localBooleans) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalBoolean.EVENT_STATE_UPDATED, this, nameof(this._OnBooleanStateUpdated));
      }

      foreach (var value in this.localFloats) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalFloat.EVENT_STATE_UPDATED, this, nameof(this._OnFloatStateUpdated));
      }

      foreach (var value in this.localInts) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalInt.EVENT_STATE_UPDATED, this, nameof(this._OnIntStateUpdated));
      }

      this._RefreshAllParameters();
    }

    private void OnDestroy() {
      foreach (var value in this.localBooleans) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }

      foreach (var value in this.localFloats) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }

      foreach (var value in this.localInts) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }
    }

    private void _RefreshAllParameters() {
      this._ApplyUpdatedBooleanState(true);
      this._ApplyUpdatedFloatState(true);
      this._ApplyUpdatedIntState(true);
    }

    public void _OnBooleanStateUpdated() { this._ApplyUpdatedBooleanState(false); }

    private void _ApplyUpdatedBooleanState(bool force) {
      for (var i = 0; i < this.localBooleans.Length; i++) {
        var value = this.localBooleans[i];
        var parameterId = this._localBooleanParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;
        if (parameterId == -1) continue;

        this.target.SetBool(parameterId, value.State);
      }
    }

    public void _OnFloatStateUpdated() { this._ApplyUpdatedFloatState(false); }

    private void _ApplyUpdatedFloatState(bool force) {
      for (var i = 0; i < this.localFloats.Length; i++) {
        var value = this.localFloats[i];
        var parameterId = this._localFloatParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;
        if (parameterId == -1) continue;

        this.target.SetFloat(parameterId, value.State);
      }
    }

    public void _OnIntStateUpdated() { this._ApplyUpdatedIntState(false); }

    private void _ApplyUpdatedIntState(bool force) {
      for (var i = 0; i < this.localInts.Length; i++) {
        var value = this.localInts[i];
        var parameterId = this._localIntParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;
        if (parameterId == -1) continue;

        this.target.SetInteger(parameterId, value.State);
      }
    }
  }
}
