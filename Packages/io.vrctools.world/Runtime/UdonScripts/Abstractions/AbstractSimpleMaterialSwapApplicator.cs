using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.LocalValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Abstractions {
  public abstract class AbstractSimpleMaterialSwapApplicator : UdonSharpBehaviour {
    public Renderer[] renderers;
    public int[] slotIndices;
    public Material[] replacementMaterials;

    public bool invert;

    private Material[] _originalMaterials;

    protected abstract bool State { get; }

    protected virtual void Start() {
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
    }

    public void _OnStateUpdated() {
      var value = this.State ^ this.invert;

      for (var i = 0; i < this.renderers.Length; i++) {
        var r = this.renderers[i];
        var slot = this.slotIndices[i];
        var replacementMaterial = this.replacementMaterials[slot];
        var originalMaterial = this._originalMaterials[slot];

        if (!Utilities.IsValid(r) || slot < 0) {
          continue;
        }

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
