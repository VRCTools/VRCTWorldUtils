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
using VRC.SDKBase;
using VRC.Udon;
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Converters
{
  /// <summary>
  /// Remaps a float value based on a pre-defined curve.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Converters/Float Curve Converter")]
  public class FloatCurveConverter : UdonSharpBehaviour
  {
    public ValueType input;
    public LocalFloat localInput;
    public SynchronizedFloat synchronizedInput;

    public AnimationCurve curve = new AnimationCurve(
      new Keyframe(0, 0),
      new Keyframe(1, 1));

    public ValueType output;
    public LocalFloat localOutput;
    public SynchronizedFloat synchronizedOutput;

    private void Start()
    {
      if (!ValueUtility.IsValid(this.input, this.localInput, this.synchronizedInput))
      {
        UdonDebug.Error(this, "Input value is invalid - Disabled");
        this.enabled = false;
        return;
      }

      if (!ValueUtility.IsValid(this.output, this.localOutput, this.synchronizedOutput))
      {
        UdonDebug.Error(this, "Output value is invalid - Disabled");
        this.enabled = false;
        return;
      }

      ValueUtility.RegisterUpdateHandler(this.input, this.localInput, this.synchronizedInput, this,
        nameof(this._OnStateUpdated));

      this._OnStateUpdated();
    }

    [CustomEvent]
    public void _OnStateUpdated()
    {
      var newValue = ValueUtility.GetValue(this.input, this.localInput, this.synchronizedInput);
      newValue = this.curve.Evaluate(newValue);
      ValueUtility.SetValue(this.output, this.localOutput, this.synchronizedOutput, newValue);
    }
  }
}
