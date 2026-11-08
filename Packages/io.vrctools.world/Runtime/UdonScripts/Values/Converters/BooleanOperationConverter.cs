// Copyright 2025-2026 .start <https://dotstart.tv>
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
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;
using VRCTools.World.Attributes;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Converters
{
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Converters/Boolean Operation Converter")]
  [ValueComponentKind(ValueComponentKind.Converter)]
  public class BooleanOperationConverter : UdonSharpBehaviour
  {
    [ValueBinding(ValueBindingType.Output,
      localFieldName: nameof(localTarget),
      synchronizedFieldName: nameof(synchronizedTarget))]
    public ValueKind target;

    public LocalBoolean localTarget;
    public SynchronizedBoolean synchronizedTarget;

    public BooleanOperation operation;

    [ValueBinding(ValueBindingType.Input,
      localFieldName: nameof(localAlphaValue),
      synchronizedFieldName: nameof(synchronizedAlphaValue))]
    public ValueSource sourceAlpha;

    public bool staticAlphaValue;
    public LocalBoolean localAlphaValue;
    public SynchronizedBoolean synchronizedAlphaValue;

    [ValueBinding(ValueBindingType.Input,
      localFieldName: nameof(localBetaValue),
      synchronizedFieldName: nameof(synchronizedBetaValue))]
    public ValueSource sourceBeta;

    public bool staticBetaValue;
    public LocalBoolean localBetaValue;
    public SynchronizedBoolean synchronizedBetaValue;

    private void Start()
    {
      if (!ValueUtility.IsValid(this.target, this.localTarget, this.synchronizedTarget))
      {
        UdonDebug.Error(this, "Target value is invalid - Disabled");
        this.enabled = false;
        return;
      }

      this._RegisterHandler(this.sourceAlpha, this.localAlphaValue, this.synchronizedAlphaValue);
      this._RegisterHandler(this.sourceBeta, this.localBetaValue, this.synchronizedBetaValue);

      this._OnComponentUpdated();
    }

    private void _RegisterHandler(
      ValueSource source,
      LocalBoolean localValue,
      SynchronizedBoolean synchronizedValue)
    {
      ValueUtility.RegisterUpdateHandler(source, localValue, synchronizedValue, this, nameof(this._OnComponentUpdated));
    }

    [CustomEvent]
    public void _OnComponentUpdated()
    {
      var alpha = ValueUtility.GetValue(this.sourceAlpha, this.staticAlphaValue, this.localAlphaValue,
        this.synchronizedAlphaValue);
      var beta = ValueUtility.GetValue(this.sourceBeta, this.staticBetaValue, this.localBetaValue,
        this.synchronizedBetaValue);

      var newValue = false;
      switch (this.operation)
      {
        case BooleanOperation.Not:
          newValue = !alpha;
          break;
        case BooleanOperation.And:
          newValue = alpha && beta;
          break;
        case BooleanOperation.Or:
          newValue = alpha || beta;
          break;
        case BooleanOperation.Xor:
          newValue = alpha ^ beta;
          break;
        case BooleanOperation.Nand:
          newValue = !(alpha || beta);
          break;
        case BooleanOperation.Nor:
          newValue = !(alpha || beta);
          break;
        case BooleanOperation.Xnor:
          newValue = !(alpha ^ beta);
          break;
      }

      ValueUtility.SetValue(this.target, this.localTarget, this.synchronizedTarget, newValue);
    }
  }
}
