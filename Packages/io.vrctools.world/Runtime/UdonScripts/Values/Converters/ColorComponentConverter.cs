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
  /// <summary>
  ///   Encapsulates a synchronized boolean value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Values/Converters/Color Component Converter")]
  public class ColorComponentConverter : UdonSharpBehaviour {
    public ValueType target;
    public LocalColor localTarget;
    public SynchronizedColor synchronizedTarget;

    public ValueSource redSource;
    public float staticRedValue = 1f;
    public LocalFloat localRedValue;
    public SynchronizedFloat synchronizedRedValue;

    public ValueSource greenSource;
    public float staticGreenValue = 1f;
    public LocalFloat localGreenValue;
    public SynchronizedFloat synchronizedGreenValue;

    public ValueSource blueSource;
    public float staticBlueValue = 1f;
    public LocalFloat localBlueValue;
    public SynchronizedFloat synchronizedBlueValue;

    public ValueSource alphaSource;
    public float staticAlphaValue = 1f;
    public LocalFloat localAlphaValue;
    public SynchronizedFloat synchronizedAlphaValue;

    private void Start() {
      if (!ValueUtility.IsValid(this.target, this.localTarget, this.synchronizedTarget)) {
        Debug.LogError("[Color Component Converter] Target value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._RegisterHandler("Red", this.redSource, this.localRedValue, this.synchronizedRedValue);
      this._RegisterHandler("Green", this.greenSource, this.localGreenValue, this.synchronizedGreenValue);
      this._RegisterHandler("Blue", this.blueSource, this.localBlueValue, this.synchronizedBlueValue);
      this._RegisterHandler("Alpha", this.alphaSource, this.localAlphaValue, this.synchronizedAlphaValue);

      this._OnComponentUpdated();
    }

    private void _RegisterHandler(
      string channel,
      ValueSource source,
      LocalFloat localValue,
      SynchronizedFloat synchronizedValue) {
      if (!ValueUtility.IsValid(source, localValue, synchronizedValue))
        Debug.LogError($"[Color Component Converter] {channel} source is invalid - Component will be set to zero",
          this);

      ValueUtility.RegisterUpdateHandler(source, localValue, synchronizedValue, this, nameof(this._OnComponentUpdated));
    }

    public void _OnComponentUpdated() {
      var red = ValueUtility.GetValue(this.redSource, this.staticRedValue, this.localRedValue,
        this.synchronizedRedValue);
      var green = ValueUtility.GetValue(this.greenSource, this.staticGreenValue, this.localGreenValue,
        this.synchronizedGreenValue);
      var blue = ValueUtility.GetValue(this.blueSource, this.staticBlueValue, this.localBlueValue,
        this.synchronizedBlueValue);
      var alpha = ValueUtility.GetValue(this.alphaSource, this.staticAlphaValue, this.localAlphaValue,
        this.synchronizedAlphaValue);

      var color = new Color(red, green, blue, alpha);
      ValueUtility.SetValue(this.target, this.localTarget, this.synchronizedTarget, color);
    }
  }
}
