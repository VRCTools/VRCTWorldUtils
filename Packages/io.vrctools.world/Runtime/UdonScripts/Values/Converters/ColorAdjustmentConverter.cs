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
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Converters {
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Values/Converters/Color Adjustment Converter")]
  public class ColorAdjustmentConverter : UdonSharpBehaviour {
    public ValueType source;
    public LocalColor localSource;
    public SynchronizedColor synchronizedSource;

    public ValueType target;
    public LocalColor localTarget;
    public SynchronizedColor synchronizedTarget;

    [Range(-1, 1)]
    public float hueOffset;
    [Range(-1, 1)]
    public float saturationOffset;
    [Range(-1, 1)]
    public float valueOffset;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localSource, this.synchronizedSource)) {
        Debug.LogError("[Color Adjustment Converter] Source is not valid - Disabled");
        this.enabled = false;
        return;
      }

      if (!ValueUtility.IsValid(this.target, this.localTarget, this.synchronizedTarget)) {
        Debug.LogError("[Color Adjustment Converter] Target is not valid - Disabled");
        this.enabled = false;
        return;
      }

      ValueUtility.RegisterUpdateHandler(this.source, this.localSource, this.synchronizedSource, this,
        nameof(this._OnSourceUpdated));

      this.SendCustomEventDelayedFrames(nameof(this._OnSourceUpdated), 1);
    }

    public void _OnSourceUpdated() {
      var color = ValueUtility.GetValue(this.source, this.localSource, this.synchronizedSource);

      Color.RGBToHSV(color, out var h, out var s, out var v);

      h += this.hueOffset;
      s = Mathf.Clamp01(s + this.saturationOffset);
      v = Mathf.Clamp01(v + this.valueOffset);

      color = Color.HSVToRGB(h, s, v);
      ValueUtility.SetValue(this.target, this.localTarget, this.synchronizedTarget, color);
    }
  }
}
