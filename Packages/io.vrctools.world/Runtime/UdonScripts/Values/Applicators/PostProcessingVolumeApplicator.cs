// Copyright 2026 .start <https://dotstart.tv>
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
using UnityEngine.Rendering.PostProcessing;
using VRC.SDKBase;
using VRCTools.Common.Utils;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Applicators
{
  /// <summary>
  /// Applies post processing volume parameters.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Applicators/Post Processing Volume Applicator")]
  public class PostProcessingVolumeApplicator : UdonSharpBehaviour
  {
    public PostProcessVolume volume;

    public ValueType weight;
    public LocalFloat localWeight;
    public SynchronizedFloat synchronizedWeight;

    private void Start()
    {
      if (!Utilities.IsValid(this.volume))
      {
        UdonDebug.Error(this, "Post process volume is invalid - Disabled");
        this.enabled = false;
        return;
      }

      if (!ValueUtility.IsValid(this.weight, this.localWeight, this.synchronizedWeight))
      {
        UdonDebug.Error(this, "Weight value is invalid - Disabled");
        this.enabled = false;
        return;
      }

      ValueUtility.RegisterUpdateHandler(this.weight, this.localWeight, this.synchronizedWeight, this,
        nameof(this._OnWeightUpdated));

      this._OnWeightUpdated();
    }

    public void _OnWeightUpdated()
    {
      var weight = ValueUtility.GetValue(this.weight, this.localWeight, this.synchronizedWeight);
      this.volume.weight = Mathf.Clamp01(weight);
    }
  }
}
