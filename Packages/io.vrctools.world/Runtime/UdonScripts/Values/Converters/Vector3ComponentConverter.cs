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
  /// Assembles a Vector3 value based on its components.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Converters/Vector3 Component Converter")]
  public class Vector3ComponentConverter : UdonSharpBehaviour
  {
    public ValueType output;
    public LocalVector3 localOutput;
    public SynchronizedVector3 synchronizedOutput;

    public ValueSource a;
    public float staticA;
    public LocalFloat localA;
    public SynchronizedFloat synchronizedA;

    public ValueSource b;
    public float staticB;
    public LocalFloat localB;
    public SynchronizedFloat synchronizedB;

    public ValueSource c;
    public float staticC;
    public LocalFloat localC;
    public SynchronizedFloat synchronizedC;

    private void Start()
    {
      if (!ValueUtility.IsValid(this.output, this.localOutput, this.synchronizedOutput))
      {
        UdonDebug.Error(this, "Output value is invalid - Disabled");
        this.enabled = false;
        return;
      }

      this._RegisterHandler("A", this.a, this.localA, this.synchronizedA);
      this._RegisterHandler("B", this.b, this.localB, this.synchronizedB);
      this._RegisterHandler("C", this.c, this.localC, this.synchronizedC);

      this._OnComponentUpdated();
    }

    private void _RegisterHandler(string component, ValueSource source, LocalFloat localValue,
      SynchronizedFloat synchronizedValue)
    {
      if (!ValueUtility.IsValid(source, localValue, synchronizedValue))
      {
        UdonDebug.Error(this, $"{component} value is invalid - Component will be set to zero");
      }

      ValueUtility.RegisterUpdateHandler(source, localValue, synchronizedValue, this, nameof(this._OnComponentUpdated));
    }

    [CustomEvent]
    public void _OnComponentUpdated()
    {
      var a = ValueUtility.GetValue(this.a, this.staticA, this.localA, this.synchronizedA);
      var b = ValueUtility.GetValue(this.b, this.staticB, this.localB, this.synchronizedB);
      var c = ValueUtility.GetValue(this.c, this.staticC, this.localC, this.synchronizedC);

      var vector = new Vector3(a, b, c);
      ValueUtility.SetValue(this.output, this.localOutput, this.synchronizedOutput, vector);
    }
  }
}
