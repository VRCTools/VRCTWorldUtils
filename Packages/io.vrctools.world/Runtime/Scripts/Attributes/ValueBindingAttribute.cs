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
using System.Reflection;
using VRCTools.World.Values;

namespace VRCTools.World.Attributes
{
  /// <summary>
  /// Annotates a given field as a value binding.
  ///
  /// This attribute may be placed on either a <see cref="ValueKind"/>, <see cref="ValueSource"/>, Local*Value,
  /// Persisted*Value or Synchronized*Value field.
  ///
  /// Note: When placing this attribute on a value, the field name parameters must be left empty.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field)]
  public sealed class ValueBindingAttribute : Attribute
  {
    /// <summary>
    /// Describes the way that a value is interacted with within the annotated component.
    /// </summary>
    public readonly ValueBindingType bindingType;

    #region Field Names

    /// <summary>
    /// Specifies the field which references the local value (only applicable when present on a <see cref="ValueKind"/>
    /// or <see cref="ValueSource"/> field).
    /// </summary>
    public readonly string localFieldName;

    /// <summary>
    /// Specifies the field which references the synchronized value (only applicable when present on a
    /// <see cref="ValueKind"/> or <see cref="ValueSource"/> field).
    /// </summary>
    public readonly string synchronizedFieldName;

    #endregion

    public ValueBindingAttribute(ValueBindingType bindingType, string localFieldName = null,
      string synchronizedFieldName = null)
    {
      this.bindingType = bindingType;

      this.localFieldName = localFieldName;
      this.synchronizedFieldName = synchronizedFieldName;
    }

#if UNITY_EDITOR && !COMPILER_UDONSHARP

    /// <summary>
    /// Retrieves the actual backing field based on the configuration of this annotation.
    /// </summary>
    /// <param name="component">a component instance</param>
    /// <param name="annotatedField">an annotated field</param>
    /// <returns>a backing field or null</returns>
    /// <exception cref="ArgumentOutOfRangeException">when an unknown value kind or source is encountered</exception>
    public FieldInfo GetBackingField(object component, FieldInfo annotatedField)
    {
      var fieldType = annotatedField.FieldType;

      var isValueSource = typeof(ValueSource).IsAssignableFrom(fieldType);
      var isValueKind = typeof(ValueKind).IsAssignableFrom(fieldType);

      if (!isValueSource && !isValueKind)
      {
        return annotatedField;
      }

      string referencedFieldName;
      if (isValueSource)
      {
        var valueSource = (ValueSource)annotatedField.GetValue(component);

        switch (valueSource)
        {
          case ValueSource.Static:
            return null;
          case ValueSource.Local:
            referencedFieldName = this.localFieldName;
            break;
          case ValueSource.Synchronized:
            referencedFieldName = this.synchronizedFieldName;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
      else
      {
        var valueKind = (ValueKind)annotatedField.GetValue(component);

        switch (valueKind)
        {
          case ValueKind.Local:
            referencedFieldName = this.localFieldName;
            break;
          case ValueKind.Synchronized:
            referencedFieldName = this.synchronizedFieldName;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }

      if (referencedFieldName == null)
      {
        return null;
      }

      return component.GetType().GetField(referencedFieldName, BindingFlags.Instance | BindingFlags.Public);
    }
#endif
  }
}
