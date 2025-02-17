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
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.SynchronizedValues.Applicators {
  /// <summary>
  ///   Applies a set of synchronized values to material parameters.
  ///   This script can run in either traditional material mode (e.g. all changes are directly applied to a given
  ///   material) or instanced mode (e.g. all changes are applied to property blocks on a given renderer).
  ///   Please note that instanced mode is only supported for materials which make use of a shader which has explicitly
  ///   been written to accept instanced properties.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/Applicators/Synchronized Material Property Applicator")]
  public class SynchronizedMaterialPropertyApplicator : UdonSharpBehaviour {
    public MaterialPropertyTarget target;

    [FormerlySerializedAs("material")]
    public Material targetMaterial;

    public Renderer targetRenderer;

    public SynchronizedBoolean[] synchronizedBooleans;
    public string[] synchronizedBooleanParameters;

    public SynchronizedColor[] synchronizedColors;
    public string[] synchronizedColorParameters;

    public SynchronizedFloat[] synchronizedFloats;
    public string[] synchronizedFloatParameters;

    public SynchronizedInt[] synchronizedInts;
    public string[] synchronizedIntParameters;

    public SynchronizedVector3[] synchronizedVectors;
    public string[] synchronizedVectorParameters;

    private MaterialPropertyBlock _materialPropertyBlock;

    private int[] _synchronizedBooleanParameterIds;
    private int[] _synchronizedColorParameterIds;
    private int[] _synchronizedFloatParameterIds;
    private int[] _synchronizedIntParameterIds;
    private int[] _synchronizedVectorParameterIds;

    private void Start() {
      if (!MaterialPropertyUtility.IsValid(this.target, this.targetMaterial, this.targetRenderer)) {
        Debug.LogError("[Synchronized Material Property Applicator] Target is invalid - Disabled");
        this.enabled = false;
      }

      // before we do anything we check for corrupted component data (e.g. if our editor has broken somehow or Unity
      // screwed up)
      this.synchronizedBooleans
        = MappingUtility.CheckCoherentMapping(this.synchronizedBooleans, this.synchronizedBooleanParameters, this);
      this.synchronizedColors
        = MappingUtility.CheckCoherentMapping(this.synchronizedColors, this.synchronizedColorParameters, this);
      this.synchronizedFloats
        = MappingUtility.CheckCoherentMapping(this.synchronizedFloats, this.synchronizedFloatParameters, this);
      this.synchronizedInts
        = MappingUtility.CheckCoherentMapping(this.synchronizedInts, this.synchronizedIntParameters, this);
      this.synchronizedVectors
        = MappingUtility.CheckCoherentMapping(this.synchronizedVectors, this.synchronizedVectorParameters, this);

      // for performance reasons we'll resolve all parameters to their indices as well
      this._synchronizedBooleanParameterIds
        = MaterialPropertyUtility.ResolveParameterIds(this.synchronizedBooleanParameters);
      this._synchronizedColorParameterIds
        = MaterialPropertyUtility.ResolveParameterIds(this.synchronizedColorParameters);
      this._synchronizedFloatParameterIds
        = MaterialPropertyUtility.ResolveParameterIds(this.synchronizedFloatParameters);
      this._synchronizedIntParameterIds = MaterialPropertyUtility.ResolveParameterIds(this.synchronizedIntParameters);
      this._synchronizedVectorParameterIds
        = MaterialPropertyUtility.ResolveParameterIds(this.synchronizedVectorParameters);

      // we handle each value separately to minimize unnecessary updates to materials which could potentially cause
      // some congestion for more frequently  synchronized values
      foreach (var value in this.synchronizedBooleans) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(SynchronizedBoolean.EVENT_STATE_UPDATED, this, nameof(this._OnBooleanStateUpdated));
      }

      foreach (var value in this.synchronizedColors) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(SynchronizedColor.EVENT_STATE_UPDATED, this, nameof(this._OnColorStateUpdated));
      }

      foreach (var value in this.synchronizedFloats) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(SynchronizedFloat.EVENT_STATE_UPDATED, this, nameof(this._OnFloatStateUpdated));
      }

      foreach (var value in this.synchronizedInts) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(SynchronizedInt.EVENT_STATE_UPDATED, this, nameof(this._OnIntStateUpdated));
      }

      foreach (var value in this.synchronizedVectors) {
        if (!Utilities.IsValid(value)) continue;

        value._RegisterHandler(SynchronizedVector3.EVENT_STATE_UPDATED, this, nameof(this._OnVectorStateUpdated));
      }

      this.SendCustomEventDelayedFrames(nameof(this._RefreshAllParameters), 1);
    }

    private void OnDestroy() {
      foreach (var value in this.synchronizedBooleans) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }

      foreach (var value in this.synchronizedColors) {
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

      foreach (var value in this.synchronizedVectors) {
        if (!Utilities.IsValid(value)) continue;

        value._UnregisterHandler(this);
      }
    }

    public void _RefreshAllParameters() {
      this._ApplyUpdatedBooleanState(true);
      this._ApplyUpdatedColorState(true);
      this._ApplyUpdatedFloatState(true);
      this._ApplyUpdatedIntState(true);
      this._ApplyUpdatedVectorState(true);
    }

    private void _EnsureMaterialBlock() {
      // for convenience this method is called every time we update a given type of parameter - if material blocks are
      // disabled, we'll just NOOP
      if (this.target != MaterialPropertyTarget.PROPERTY_BLOCK) return;

      // allocation of material property blocks in Start() used to cause Unity to crash in some circumstances thus
      // requiring this delayed allocation
      if (this._materialPropertyBlock == null) this._materialPropertyBlock = new MaterialPropertyBlock();

      // we perform partial updates meaning only properties which have actually changed will be considered - pull the
      // current values from the renderer to prevent accidental overwriting of unaffected as well as unchanged
      // properties
      this.targetRenderer.GetPropertyBlock(this._materialPropertyBlock);
    }

    private void _ApplyMaterialBlock() {
      // for convenience this method is called every time we update a given type of parameter - if material blocks are
      // disabled, we'll just NOOP
      if (this.target != MaterialPropertyTarget.PROPERTY_BLOCK) return;

      this.targetRenderer.SetPropertyBlock(this._materialPropertyBlock);
    }

    public void _OnBooleanStateUpdated() { this._ApplyUpdatedBooleanState(false); }

    private void _ApplyUpdatedBooleanState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.synchronizedBooleans.Length; i++) {
        var value = this.synchronizedBooleans[i];
        var parameterId = this._synchronizedBooleanParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;

        MaterialPropertyUtility.SetParameter(this.target, this.targetMaterial, this._materialPropertyBlock, parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnColorStateUpdated() { this._ApplyUpdatedColorState(false); }

    private void _ApplyUpdatedColorState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.synchronizedColors.Length; ++i) {
        var value = this.synchronizedColors[i];
        var parameterId = this._synchronizedColorParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;

        MaterialPropertyUtility.SetParameter(this.target, this.targetMaterial, this._materialPropertyBlock, parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnFloatStateUpdated() { this._ApplyUpdatedFloatState(false); }

    private void _ApplyUpdatedFloatState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.synchronizedFloats.Length; ++i) {
        var value = this.synchronizedFloats[i];
        var parameterId = this._synchronizedFloatParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;

        MaterialPropertyUtility.SetParameter(this.target, this.targetMaterial, this._materialPropertyBlock, parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnIntStateUpdated() { this._ApplyUpdatedIntState(false); }

    private void _ApplyUpdatedIntState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.synchronizedInts.Length; ++i) {
        var value = this.synchronizedInts[i];
        var parameterId = this._synchronizedIntParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;

        MaterialPropertyUtility.SetParameter(this.target, this.targetMaterial, this._materialPropertyBlock, parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }

    public void _OnVectorStateUpdated() { this._ApplyUpdatedVectorState(false); }

    private void _ApplyUpdatedVectorState(bool force) {
      this._EnsureMaterialBlock();

      for (var i = 0; i < this.synchronizedVectors.Length; ++i) {
        var value = this.synchronizedVectors[i];
        var parameterId = this._synchronizedVectorParameterIds[i];
        if (!force && !value.IsUpdatingHandlers) continue;

        MaterialPropertyUtility.SetParameter(this.target, this.targetMaterial, this._materialPropertyBlock, parameterId, value.State);
      }

      this._ApplyMaterialBlock();
    }
  }
}
