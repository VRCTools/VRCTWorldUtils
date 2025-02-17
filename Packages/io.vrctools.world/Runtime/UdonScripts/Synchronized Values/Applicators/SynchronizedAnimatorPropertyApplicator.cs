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

namespace VRCTools.World.SynchronizedValues.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/Applicators/Synchronized Animator Property Applicator")]
  public class SynchronizedAnimatorPropertyApplicator : UdonSharpBehaviour {
    public Animator target;

    public SynchronizedBoolean[] synchronizedBooleans;
    public string[] synchronizedBooleanParameters;

    public SynchronizedFloat[] synchronizedFloats;
    public string[] synchronizedFloatParameters;

    public SynchronizedInt[] synchronizedInts;
    public string[] synchronizedIntParameters;

    private int[] _synchronizedBooleanParameterIds;
    private int[] _synchronizedFloatParameterIds;
    private int[] _synchronizedIntParameterIds;

    private void Start() {
      if (!Utilities.IsValid(this.target)) {
        Debug.LogError("Synchronized Animator Property Applicator] Animator is invalid - Disabled");
        this.enabled = false;
      }

      this.synchronizedBooleans = MappingUtility.CheckCoherentMapping(this.synchronizedBooleans, this.synchronizedBooleanParameters, this);
      this.synchronizedFloats = MappingUtility.CheckCoherentMapping(this.synchronizedFloats, this.synchronizedFloatParameters, this);
      this.synchronizedInts = MappingUtility.CheckCoherentMapping(this.synchronizedInts, this.synchronizedIntParameters, this);

      this._synchronizedBooleanParameterIds = AnimatorPropertyUtility.ResolveParameterIds(this.synchronizedBooleanParameters);
      this._synchronizedFloatParameterIds = AnimatorPropertyUtility.ResolveParameterIds(this.synchronizedFloatParameters);
      this._synchronizedIntParameterIds = AnimatorPropertyUtility.ResolveParameterIds(this.synchronizedIntParameters);

      foreach (var value in this.synchronizedBooleans) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(SynchronizedBoolean.EVENT_STATE_UPDATED, this, nameof(this._OnBooleanStateUpdated));
      }

      foreach (var value in this.synchronizedFloats) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(SynchronizedFloat.EVENT_STATE_UPDATED, this, nameof(this._OnFloatStateUpdated));
      }

      foreach (var value in this.synchronizedInts) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(SynchronizedInt.EVENT_STATE_UPDATED, this, nameof(this._OnIntStateUpdated));
      }

      this._RefreshAllParameters();
    }

    private void OnDestroy() {
      foreach (var value in this.synchronizedBooleans) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }

      foreach (var value in this.synchronizedFloats) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }

      foreach (var value in this.synchronizedInts) {
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
      for (var i = 0; i < this.synchronizedBooleans.Length; i++) {
        var value = this.synchronizedBooleans[i];
        var parameterId = this._synchronizedBooleanParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;
        if (parameterId == -1) continue;

        this.target.SetBool(parameterId, value.State);
      }
    }

    public void _OnFloatStateUpdated() { this._ApplyUpdatedFloatState(false); }

    private void _ApplyUpdatedFloatState(bool force) {
      for (var i = 0; i < this.synchronizedFloats.Length; i++) {
        var value = this.synchronizedFloats[i];
        var parameterId = this._synchronizedFloatParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;
        if (parameterId == -1) continue;

        this.target.SetFloat(parameterId, value.State);
      }
    }

    public void _OnIntStateUpdated() { this._ApplyUpdatedIntState(false); }

    private void _ApplyUpdatedIntState(bool force) {
      for (var i = 0; i < this.synchronizedInts.Length; i++) {
        var value = this.synchronizedInts[i];
        var parameterId = this._synchronizedIntParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;
        if (parameterId == -1) continue;

        this.target.SetInteger(parameterId, value.State);
      }
    }
  }
}
