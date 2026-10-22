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

namespace VRCTools.World.Values.Applicators
{
  /// <summary>
  /// Applies transforms based on values.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Applicators/Transform Applicator")]
  public class TransformApplicator : UdonSharpBehaviour
  {
    public TransformSpace positionTransformSpace = TransformSpace.Local;
    public FractionalMathOperation positionOperation;
    public ValueType positionModifier;
    public LocalVector3 localPositionModifier;
    public SynchronizedVector3 synchronizedPositionModifier;

    public TransformSpace rotationTransformSpace = TransformSpace.Local;
    public FractionalMathOperation rotationOperation;
    public ValueType rotationModifier;
    public LocalVector3 localRotationModifier;
    public SynchronizedVector3 synchronizedRotationModifier;

    public FractionalMathOperation scaleOperation;
    public ValueType scaleModifier;
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
        case FractionalMathOperation.ADD:
          position += value;
          break;
        case FractionalMathOperation.SUBTRACT:
          position -= value;
          break;
        case FractionalMathOperation.MULTIPLY:
          position.Scale(value);
          break;
        case FractionalMathOperation.DIVIDE:
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
        case FractionalMathOperation.ADD:
          rotation += value;
          break;
        case FractionalMathOperation.SUBTRACT:
          rotation -= value;
          break;
        case FractionalMathOperation.MULTIPLY:
          rotation.Scale(value);
          break;
        case FractionalMathOperation.DIVIDE:
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
        case FractionalMathOperation.ADD:
          scale += value;
          break;
        case FractionalMathOperation.SUBTRACT:
          scale -= value;
          break;
        case FractionalMathOperation.MULTIPLY:
          scale.Scale(value);
          break;
        case FractionalMathOperation.DIVIDE:
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
