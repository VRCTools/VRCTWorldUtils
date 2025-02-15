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
using UnityEngine.Serialization;
using VRC.SDKBase;
using VRCTools.World.Utils;

namespace VRCTools.World.LocalValues.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Applicators/Local Material Property Applicator")]
  public class LocalMaterialPropertyApplicator : UdonSharpBehaviour {
    public Material material;

    [FormerlySerializedAs("useMaterialBlock")]
    public bool usePropertyBlock;

    public Renderer targetRenderer;

    public LocalBoolean[] localBooleans;
    public string[] localBooleanParameters;

    public LocalColor[] localColors;
    public string[] localColorParameters;

    public LocalFloat[] localFloats;
    public string[] localFloatParameters;

    public LocalInt[] localInts;
    public string[] localIntParameters;

    public LocalTexture2D[] localTextures;
    public string[] localTextureParameters;

    public LocalVector3[] localVectors;
    public string[] localVectorParameters;

    private int[] _localBooleanParameterIds;
    private int[] _localColorParameterIds;
    private int[] _localFloatParameterIds;
    private int[] _localIntParameterIds;
    private int[] _localTextureParameterIds;
    private int[] _localVectorParameterIds;

    private MaterialPropertyBlock _materialPropertyBlock;

    private void Start() {
      if (this.usePropertyBlock) {
        if (!Utilities.IsValid(this.targetRenderer)) {
          Debug.LogError("[Local Material Property Applicator] Renderer is invalid - Disabled");
          this.enabled = false;
        }
      } else if (!Utilities.IsValid(this.material)) {
        Debug.LogError("[Local Material Property Applicator] Material is invalid - Disabled");
        this.enabled = false;
      }

      // before we do anything we check for corrupted component data (e.g. if our editor has broken somehow or Unity
      // screwed up)
      this.localBooleans
        = MappingUtility.CheckCoherentMapping(this.localBooleans, this.localBooleanParameters, this);
      this.localColors
        = MappingUtility.CheckCoherentMapping(this.localColors, this.localColorParameters, this);
      this.localFloats
        = MappingUtility.CheckCoherentMapping(this.localFloats, this.localFloatParameters, this);
      this.localInts
        = MappingUtility.CheckCoherentMapping(this.localInts, this.localIntParameters, this);
      this.localTextures = MappingUtility.CheckCoherentMapping(this.localTextures, this.localTextureParameters, this);
      this.localVectors
        = MappingUtility.CheckCoherentMapping(this.localVectors, this.localVectorParameters, this);

      // for performance reasons we'll resolve all parameters to their indices as well
      this._localBooleanParameterIds = _ResolveParameterIds(this.localBooleanParameters);
      this._localColorParameterIds = _ResolveParameterIds(this.localColorParameters);
      this._localFloatParameterIds = _ResolveParameterIds(this.localFloatParameters);
      this._localIntParameterIds = _ResolveParameterIds(this.localIntParameters);
      this._localTextureParameterIds = _ResolveParameterIds(this.localTextureParameters);
      this._localVectorParameterIds = _ResolveParameterIds(this.localVectorParameters);

      // we handle each value separately to minimize unnecessary updates to materials which could potentially cause
      // some congestion for more frequently  local values
      foreach (var value in this.localBooleans) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalBoolean.EVENT_STATE_UPDATED, this, nameof(this._OnBooleanStateUpdated));
      }

      foreach (var value in this.localColors) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalColor.EVENT_STATE_UPDATED, this, nameof(this._OnColorStateUpdated));
      }

      foreach (var value in this.localFloats) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalFloat.EVENT_STATE_UPDATED, this, nameof(this._OnFloatStateUpdated));
      }

      foreach (var value in this.localInts) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalInt.EVENT_STATE_UPDATED, this, nameof(this._OnIntStateUpdated));
      }

      foreach (var value in this.localTextures) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalTexture2D.EVENT_STATE_UPDATED, this, nameof(this._OnTextureStateUpdated));
      }

      foreach (var value in this.localVectors) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(LocalVector3.EVENT_STATE_UPDATED, this, nameof(this._OnVectorStateUpdated));
      }

      this.SendCustomEventDelayedFrames(nameof(this._RefreshAllParameters), 1);
    }

    private void OnDestroy() {
      foreach (var value in this.localBooleans) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }

      foreach (var value in this.localColors) {
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

      foreach (var value in this.localTextures) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }

      foreach (var value in this.localVectors) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }
    }

    private static int[] _ResolveParameterIds(string[] parameterNames) {
      var parameterIds = new int[parameterNames.Length];
      for (var i = 0; i < parameterNames.Length; i++) {
        var parameterName = parameterNames[i];
        if (string.IsNullOrEmpty(parameterName)) {
          parameterIds[i] = -1;
          continue;
        }

        parameterIds[i] = VRCShader.PropertyToID(parameterNames[i]);
      }

      return parameterIds;
    }

    public void _RefreshAllParameters() {
      this._ApplyUpdatedBooleanState(true);
      this._ApplyUpdatedColorState(true);
      this._ApplyUpdatedFloatState(true);
      this._ApplyUpdatedIntState(true);
      this._ApplyUpdatedTextureState(true);
      this._ApplyUpdatedVectorState(true);
    }

    private void _EnsureMaterialBlock() {
      // for convenience this method is called every time we update a given type of parameter - if material blocks are
      // disabled, we'll just NOOP
      if (!this.usePropertyBlock) return;

      // allocation of material property blocks in Start() used to cause Unity to crash in some circumstances thus
      // requiring this delayed allocation
      if (this._materialPropertyBlock == null) this._materialPropertyBlock = new MaterialPropertyBlock();

      // we perform partial updates meaning only properties which have actually changed will be considered - pull the
      // current values from the renderer to prevent accidental overwriting of unaffected as well as unchanged
      // properties
      this.targetRenderer.GetPropertyBlock(this._materialPropertyBlock);
    }

    private static bool _ValidateParameter(UdonSharpBehaviour behaviour, int parameterId) {
      return Utilities.IsValid(parameterId) && parameterId != -1;
    }

    private void _ApplyMaterialBlock() {
      // for convenience this method is called every time we update a given type of parameter - if material blocks are
      // disabled, we'll just NOOP
      if (!this.usePropertyBlock) return;

      this.targetRenderer.SetPropertyBlock(this._materialPropertyBlock);
    }

    public void _OnBooleanStateUpdated() { this._ApplyUpdatedBooleanState(false); }

    private void _ApplyUpdatedBooleanState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.localBooleans.Length; i++) {
        var value = this.localBooleans[i];
        var parameterId = this._localBooleanParameterIds[i];
        if (!_ValidateParameter(value, parameterId)) continue;
        if (!force && !value.IsUpdatingHandlers) continue;

        if (this.usePropertyBlock)
          this._materialPropertyBlock.SetInt(parameterId, value.State ? 1 : 0);
        else
          this.material.SetInt(parameterId, value.State ? 1 : 0);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnColorStateUpdated() { this._ApplyUpdatedColorState(false); }

    private void _ApplyUpdatedColorState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.localColors.Length; ++i) {
        var value = this.localColors[i];
        var parameterId = this._localColorParameterIds[i];
        if (!_ValidateParameter(value, parameterId)) continue;
        if (!force && !value.IsUpdatingHandlers) continue;

        if (this.usePropertyBlock)
          this._materialPropertyBlock.SetColor(parameterId, value.State);
        else
          this.material.SetColor(parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnFloatStateUpdated() { this._ApplyUpdatedFloatState(false); }

    private void _ApplyUpdatedFloatState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.localFloats.Length; ++i) {
        var value = this.localFloats[i];
        var parameterId = this._localFloatParameterIds[i];
        if (!_ValidateParameter(value, parameterId)) continue;
        if (!force && !value.IsUpdatingHandlers) continue;

        if (this.usePropertyBlock)
          this._materialPropertyBlock.SetFloat(parameterId, value.State);
        else
          this.material.SetFloat(parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnIntStateUpdated() { this._ApplyUpdatedIntState(false); }

    private void _ApplyUpdatedIntState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.localInts.Length; ++i) {
        var value = this.localInts[i];
        var parameterId = this._localIntParameterIds[i];
        if (!_ValidateParameter(value, parameterId)) continue;
        if (!force && !value.IsUpdatingHandlers) continue;

        if (this.usePropertyBlock)
          this._materialPropertyBlock.SetInt(parameterId, value.State);
        else
          this.material.SetInt(parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnTextureStateUpdated() { this._ApplyUpdatedTextureState(false); }

    private void _ApplyUpdatedTextureState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.localTextures.Length; ++i) {
        var value = this.localTextures[i];
        var parameterId = this._localTextureParameterIds[i];
        if (!_ValidateParameter(value, parameterId)) continue;
        if (!force && !value.IsUpdatingHandlers) continue;

        if (this.usePropertyBlock)
          this._materialPropertyBlock.SetTexture(parameterId, value.State);
        else
          this.material.SetTexture(parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnVectorStateUpdated() { this._ApplyUpdatedVectorState(false); }

    private void _ApplyUpdatedVectorState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.localVectors.Length; ++i) {
        var value = this.localVectors[i];
        var parameterId = this._localVectorParameterIds[i];
        if (!_ValidateParameter(value, parameterId)) continue;
        if (!force && !value.IsUpdatingHandlers) continue;

        if (this.usePropertyBlock)
          this._materialPropertyBlock.SetVector(parameterId, value.State);
        else
          this.material.SetVector(parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }
  }
}
