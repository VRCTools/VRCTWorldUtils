using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.LocalValues;

namespace VRCTools.World.Abstractions {
  public abstract class AbstractObjectToggleApplicator : UdonSharpBehaviour {
    public bool invert;

    public GameObject[] targets;

    protected abstract bool State { get; }

    public void _OnStateUpdated() {
      var active = this.State ^ this.invert;

      foreach (var target in this.targets) {
        if (!Utilities.IsValid(target)) continue;

        target.SetActive(active);
      }
    }
  }
}
