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
using VRCTools.World.Attributes;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Applicators
{
  /// <summary>
  /// Applies transforms based on values.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Applicators/Transform Applicator")]
  [ValueComponentKind(ValueComponentKind.Applicator)]
  public class TransformApplicator : UdonSharpBehaviour
  {
    public TransformSpace positionTransformSpace = TransformSpace.Local;
    public FractionalMathOperation positionOperation;

    [ValueBinding(ValueBindingType.Input, localFieldName: nameof(localPositionModifier),
      synchronizedFieldName: nameof(synchronizedPositionModifier))]
    public ValueKind positionModifier;

    public LocalVector3 localPositionModifier;
    public SynchronizedVector3 synchronizedPositionModifier;

    public TransformSpace rotationTransformSpace = TransformSpace.Local;
    public FractionalMathOperation rotationOperation;

    [ValueBinding(ValueBindingType.Input, localFieldName: nameof(localRotationModifier),
      synchronizedFieldName: nameof(synchronizedRotationModifier))]
    public ValueKind rotationModifier;

    public LocalVector3 localRotationModifier;
    public SynchronizedVector3 synchronizedRotationModifier;

    public FractionalMathOperation scaleOperation;

    [ValueBinding(ValueBindingType.Input, localFieldName: nameof(localScaleModifier),
      synchronizedFieldName: nameof(synchronizedScaleModifier))]
    public ValueKind scaleModifier;

    public LocalVector3 localScaleModifier;
    public SynchronizedVector3 synchronizedScaleModifier;

    private Vector3 _initialPosition;
    private Vector3 _initialRotation;
    private Vector3 _initialScale;

    private void Start()
    {
      if (ValueUtility.IsValid(this.positionModifier, this.localPositionModifier, this.synchronizedPositionModifier))
      {
        switch (this.positionTransformSpace)
        {
          case TransformSpace.Local:
            this._initialPosition = this.transform.localPosition;
            break;
          case TransformSpace.Global:
            this._initialPosition = this.transform.position;
            break;
          default:
            UdonDebug.Error(this, $"Transform space mode is unsupported: {this.positionTransformSpace}");
            break;
        }

        ValueUtility.RegisterUpdateHandler(this.positionModifier, this.localPositionModifier,
          this.synchronizedPositionModifier, this, nameof(this._OnPositionUpdated));

        this._OnPositionUpdated();
      }

      if (ValueUtility.IsValid(this.rotationModifier, this.localRotationModifier, this.synchronizedRotationModifier))
      {
        switch (this.rotationTransformSpace)
        {
          case TransformSpace.Local:
            this._initialRotation = this.transform.localRotation.eulerAngles;
            break;
          case TransformSpace.Global:
            this._initialRotation = this.transform.rotation.eulerAngles;
            break;
          default:
            UdonDebug.Error(this, $"Transform space mode is unsupported: {this.rotationTransformSpace}");
            break;
        }

        ValueUtility.RegisterUpdateHandler(this.rotationModifier, this.localRotationModifier,
          this.synchronizedRotationModifier, this, nameof(this._OnRotationUpdated));

        this._OnRotationUpdated();
      }

      if (ValueUtility.IsValid(this.scaleModifier, this.localScaleModifier, this.synchronizedScaleModifier))
      {
        this._initialScale = this.transform.localScale;

        ValueUtility.RegisterUpdateHandler(this.scaleModifier, this.localScaleModifier, this.synchronizedScaleModifier,
          this, nameof(this._OnScaleUpdated));

        this._OnScaleUpdated();
      }
    }

    [CustomEvent]
    public void _OnPositionUpdated()
    {
      var position = this._initialPosition;
      var value = ValueUtility.GetValue(this.positionModifier, this.localPositionModifier,
        this.synchronizedPositionModifier);

      switch (this.positionOperation)
      {
        case FractionalMathOperation.Add:
          position += value;
          break;
        case FractionalMathOperation.Subtract:
          position -= value;
          break;
        case FractionalMathOperation.Multiply:
          position.Scale(value);
          break;
        case FractionalMathOperation.Divide:
          position.x /= value.x;
          position.y /= value.y;
          position.z /= value.z;
          break;
        default:
          UdonDebug.Error(this, $"Position operation is unsupported: {this.positionOperation} - Value unchanged");
          return;
      }

      switch (this.positionTransformSpace)
      {
        case TransformSpace.Local:
          this.transform.localPosition = position;
          break;
        case TransformSpace.Global:
          this.transform.position = position;
          break;
        default:
          UdonDebug.Error(this,
            $"Transform space mode is unsupported: {this.positionTransformSpace} - Value unchanged");
          break;
      }
    }

    [CustomEvent]
    public void _OnRotationUpdated()
    {
      var rotation = this._initialRotation;
      var value = ValueUtility.GetValue(this.rotationModifier, this.localRotationModifier,
        this.synchronizedRotationModifier);

      switch (this.rotationOperation)
      {
        case FractionalMathOperation.Add:
          rotation += value;
          break;
        case FractionalMathOperation.Subtract:
          rotation -= value;
          break;
        case FractionalMathOperation.Multiply:
          rotation.Scale(value);
          break;
        case FractionalMathOperation.Divide:
          rotation.x /= value.x;
          rotation.y /= value.y;
          rotation.z /= value.z;
          break;
        default:
          UdonDebug.Error(this, $"Rotation operation is unsupported: {this.rotationOperation} - Value unchanged");
          return;
      }

      switch (this.rotationTransformSpace)
      {
        case TransformSpace.Local:
          this.transform.localRotation = Quaternion.Euler(rotation);
          break;
        case TransformSpace.Global:
          this.transform.rotation = Quaternion.Euler(rotation);
          break;
        default:
          UdonDebug.Error(this,
            $"Transform space mode is unsupported: {this.rotationTransformSpace} - Value unchanged");
          break;
      }
    }

    [CustomEvent]
    public void _OnScaleUpdated()
    {
      var scale = this._initialScale;
      var value = ValueUtility.GetValue(this.scaleModifier, this.localScaleModifier, this.synchronizedScaleModifier);

      switch (this.scaleOperation)
      {
        case FractionalMathOperation.Add:
          scale += value;
          break;
        case FractionalMathOperation.Subtract:
          scale -= value;
          break;
        case FractionalMathOperation.Multiply:
          scale.Scale(value);
          break;
        case FractionalMathOperation.Divide:
          scale.x /= value.x;
          scale.y /= value.y;
          scale.z /= value.z;
          break;
        default:
          UdonDebug.Error(this, $"Scale operation is unsupported: {this.scaleOperation} - Value unchanged");
          return;
      }

      this.transform.localScale = scale;
    }
  }
}
