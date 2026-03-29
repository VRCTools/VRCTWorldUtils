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
  ///   Performs basic math operations on local or synchronized values.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Converters/Float Math Converter")]
  public class FloatMathConverter : UdonSharpBehaviour {
    public ValueType target;
    public LocalFloat localTarget;
    public SynchronizedFloat synchronizedTarget;

    public FractionalMathOperation operation;

    public ValueSource sourceA;
    public float staticValueA = 1f;
    public LocalFloat localValueA;
    public SynchronizedFloat synchronizedValueA;

    public ValueSource sourceB;
    public float staticValueB = 1f;
    public LocalFloat localValueB;
    public SynchronizedFloat synchronizedValueB;

    private void Start() {
      if (!ValueUtility.IsValid(this.target, this.localTarget, this.synchronizedTarget)) {
        Debug.LogError("[Float Math Converter] Target value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      ValueUtility.RegisterUpdateHandler(this.sourceA, this.localValueA, this.synchronizedValueA, this,
        nameof(this._OnStateUpdated));
      ValueUtility.RegisterUpdateHandler(this.sourceB, this.localValueB, this.synchronizedValueB, this,
        nameof(this._OnStateUpdated));
    }

    public void _OnStateUpdated() {
      var a = ValueUtility.GetValue(this.sourceA, this.staticValueA, this.localValueA, this.synchronizedValueA);
      var b = ValueUtility.GetValue(this.sourceB, this.staticValueB, this.localValueB, this.synchronizedValueB);

      float result;
      switch (this.operation) {
        case FractionalMathOperation.ADD:
          result = a + b;
          break;
        case FractionalMathOperation.SUBTRACT:
          result = a - b;
          break;
        case FractionalMathOperation.MULTIPLY:
          result = a * b;
          break;
        case FractionalMathOperation.DIVIDE:
          result = a / b;
          break;
        default:
          Debug.LogError("[Float Math Converter] Operation is not implemented", this);
          return;
      }

      ValueUtility.SetValue(this.target, this.localTarget, this.synchronizedTarget, result);
    }
  }
}
