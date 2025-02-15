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
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Applicators/Simple Material Swap Applicator")]
  public class SimpleMaterialSwapApplicator : UdonSharpBehaviour {
    public ValueType source;
    public LocalBoolean localValue;
    public SynchronizedBoolean synchronizedValue;

    public Renderer[] renderers;
    public int[] slotIndices;
    public Material[] replacementMaterials;

    public bool invert;

    private Material[] _originalMaterials;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localValue, this.synchronizedValue)) {
        Debug.LogError("[Simple Material Swap Applicator] Invalid value reference - Disabled", this);
        this.enabled = false;
        return;
      }

      this.renderers = MappingUtility.CheckCoherentMapping(this.renderers, this.slotIndices, this);
      this.renderers = MappingUtility.CheckCoherentMapping(this.renderers, this.replacementMaterials, this);

      this._originalMaterials = new Material[this.renderers.Length];
      for (var i = 0; i < this.renderers.Length; i++) {
        var r = this.renderers[i];
        var slot = this.slotIndices[i];

        if (slot >= r.sharedMaterials.Length) {
          Debug.LogWarning(
            $"[Simple Material Swap Applicator] Desired slot {slot} exceeds allocated material list length", r);
          continue;
        }

        this._originalMaterials[i] = r.sharedMaterials[slot];
      }

      this.localValue._RegisterHandler(LocalBoolean.EVENT_STATE_UPDATED, this, nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    public void _OnStateUpdated() {
      var value = ValueUtility.GetValue(this.source, this.localValue, this.synchronizedValue) ^ this.invert;

      for (var i = 0; i < this.renderers.Length; i++) {
        var r = this.renderers[i];
        var slot = this.slotIndices[i];
        var replacementMaterial = this.replacementMaterials[slot];
        var originalMaterial = this._originalMaterials[slot];

        if (!Utilities.IsValid(r) || slot < 0) continue;

        if (slot >= r.sharedMaterials.Length) {
          Debug.LogWarning(
            $"[Simple Material Swap Applicator] Desired slot {slot} exceeds allocated material list length", r);
          continue;
        }

        var materials = r.sharedMaterials;
        materials[i] = value ? replacementMaterial : originalMaterial;
        r.sharedMaterials = materials;
      }
    }
  }
}
