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

using System;
using UdonSharp;
using UnityEngine;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Converters {
  /// <summary>
  ///   Smoothly transitions values based on a source.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Converters/Smooth Float Converter")]
  public class SmoothFloatConverter : UdonSharpBehaviour {
    public ValueType source;
    public LocalFloat localSource;
    public SynchronizedFloat synchronizedSource;

    public ValueType target;
    public LocalFloat localTarget;
    public SynchronizedFloat synchronizedTarget;

    public float smoothTime = 1f;
    public float maxSpeed = float.MaxValue;
    public float minDifference = float.Epsilon;

    private bool _active;
    private float _velocity;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localSource, this.synchronizedSource)) {
        Debug.LogError("[Smooth Float Converter] Source value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      if (!ValueUtility.IsValid(this.target, this.localTarget, this.synchronizedTarget)) {
        Debug.LogError("[Smooth Float Converter] Target value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      ValueUtility.RegisterUpdateHandler(this.source, this.localSource, this.synchronizedSource, this,
        nameof(this._OnStateUpdated));
    }

    private void Update() {
      if (!this._active) {
        return;
      }

      var targetValue = ValueUtility.GetValue(this.source, this.localSource, this.synchronizedSource);
      var currentValue = ValueUtility.GetValue(this.target, this.localTarget, this.synchronizedTarget);
      
      // if either target value or current value become NaN or infinity for whatever reason, we will no longer be able
      // to perform smoothing and should immediately adopt the target value before de-activating ourselves
      if (float.IsNaN(targetValue) || float.IsInfinity(targetValue) || float.IsNaN(currentValue) || float.IsInfinity(currentValue))
      {
        ValueUtility.SetValue(this.target, this.localTarget, this.synchronizedTarget, targetValue);
        this._active = false;
        return;
      }

      var newValue = Mathf.SmoothDamp(currentValue, targetValue, ref this._velocity, this.smoothTime, this.maxSpeed);
      ValueUtility.SetValue(this.target, this.localTarget, this.synchronizedTarget, newValue);

      // de-activate update logic when the value gets close enough to the target so that we do not waste CPU cycles
      // doing nothing; float.Epsilon is the absolute minimum difference supported by us
      var difference = Mathf.Abs(newValue - targetValue);
      if (difference <= Mathf.Max(float.Epsilon, this.minDifference)) {
        this._active = false;
      }
    }

    public void _OnStateUpdated() {
      if (this._active)
      {
        return;
      }
      
      // this converter is special as we do not directly update the targets from here, instead this method merely
      // indicates that there is work to be done for Update() which will then slowly adjust the target until the end
      // condition is met
      this._active = true;
      this._velocity = 0f;
    }
  }
}
