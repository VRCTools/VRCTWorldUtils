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
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Converters
{
  /// <summary>
  /// Compares a int value against a given threshold to produce a boolean value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Converters/Int Comparison Converter")]
  public class IntComparisonConverter : UdonSharpBehaviour
  {
    public ValueType output;
    public LocalBoolean localOutput;
    public SynchronizedBoolean synchronizedOutput;

    public ValueSource a;
    public int staticA;
    public LocalInt localA;
    public SynchronizedInt synchronizedA;

    public ComparisonOperation operation;

    public ValueSource b;
    public int staticB;
    public LocalInt localB;
    public SynchronizedInt synchronizedB;

    private void Start()
    {
      if (!ValueUtility.IsValid(this.output, this.localOutput, this.synchronizedOutput))
      {
        UdonDebug.Error(this, "Output value is invalid - Disabled");
        this.enabled = false;
        return;
      }

      if (!ValueUtility.IsValid(this.a, this.localA, this.synchronizedA))
      {
        UdonDebug.Error(this, "A value is invalid - Disabled");
        this.enabled = false;
        return;
      }

      if (!ValueUtility.IsValid(this.b, this.localB, this.synchronizedB))
      {
        UdonDebug.Error(this, "B value is invalid - Disabled");
        this.enabled = false;
        return;
      }

      ValueUtility.RegisterUpdateHandler(this.a, this.localA, this.synchronizedA, this, nameof(this._OnStateUpdated));
      ValueUtility.RegisterUpdateHandler(this.b, this.localB, this.synchronizedB, this, nameof(this._OnStateUpdated));

      this._OnStateUpdated();
    }

    [CustomEvent]
    public void _OnStateUpdated()
    {
      var valueA = ValueUtility.GetValue(this.a, this.staticA, this.localA, this.synchronizedA);
      var valueB = ValueUtility.GetValue(this.b, this.staticB, this.localB, this.synchronizedB);

      bool result;
      switch (this.operation)
      {
        case ComparisonOperation.LessThan:
          result = valueA < valueB;
          break;
        case ComparisonOperation.LessThanOrEqualTo:
          result = valueA <= valueB;
          break;
        case ComparisonOperation.GreaterThan:
          result = valueA > valueB;
          break;
        case ComparisonOperation.GreaterThanOrEqualTo:
          result = valueA >= valueB;
          break;
        default:
          UdonDebug.Error(this, "Operation is not implemented");
          return;
      }

      ValueUtility.SetValue(this.output, this.localOutput, this.synchronizedOutput, result);
    }
  }
}
