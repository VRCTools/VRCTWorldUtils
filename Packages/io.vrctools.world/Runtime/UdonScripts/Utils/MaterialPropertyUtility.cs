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

using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.Values.Applicators;

namespace VRCTools.World.Utils {
  public static class MaterialPropertyUtility {
    public static bool IsValid(MaterialPropertyTarget target, Material material, Renderer renderer) {
      switch (target) {
        case MaterialPropertyTarget.MATERIAL:
          return Utilities.IsValid(material);
        case MaterialPropertyTarget.PROPERTY_BLOCK:
          return Utilities.IsValid(renderer);
        default:
          return true;
      }
    }

    public static int[] ResolveParameterIds(string[] parameterNames) {
      var parameterIds = new int[parameterNames.Length];
      for (var i = 0; i < parameterNames.Length; i++) {
        var parameterName = parameterNames[i];
        if (string.IsNullOrEmpty(parameterName)) {
          parameterIds[i] = -1;
          continue;
        }

        parameterIds[i] = VRCShader.PropertyToID(parameterNames[i]);
      }

      return parameterIds;
    }

    public static bool IsValidParameterId(int parameterId) { return parameterId != -1; }

    public static void SetParameter(
      MaterialPropertyTarget target,
      Material material,
      MaterialPropertyBlock propertyBlock,
      int parameterId,
      bool value) {
      if (!IsValidParameterId(parameterId)) {
        return;
      }

      switch (target) {
        case MaterialPropertyTarget.MATERIAL:
          SetParameter(material, parameterId, value);
          break;
        case MaterialPropertyTarget.PROPERTY_BLOCK:
          SetParameter(propertyBlock, parameterId, value);
          break;
        case MaterialPropertyTarget.GLOBAL:
          SetParameter(parameterId, value);
          break;
      }
    }

    public static void SetParameter(Material material, int parameterId, bool value) {
      if (!Utilities.IsValid(material)) {
        return;
      }

      material.SetInt(parameterId, value ? 1 : 0);
    }

    public static void SetParameter(MaterialPropertyBlock propertyBlock, int parameterId, bool value) {
      if (!Utilities.IsValid(propertyBlock)) {
        return;
      }

      propertyBlock.SetInt(parameterId, value ? 1 : 0);
    }

    public static void SetParameter(int parameterId, bool value) {
      VRCShader.SetGlobalInteger(parameterId, value ? 1 : 0);
    }

    public static void SetParameter(
      MaterialPropertyTarget target,
      Material material,
      MaterialPropertyBlock propertyBlock,
      int parameterId,
      Color value) {
      if (!IsValidParameterId(parameterId)) {
        return;
      }

      switch (target) {
        case MaterialPropertyTarget.MATERIAL:
          SetParameter(material, parameterId, value);
          break;
        case MaterialPropertyTarget.PROPERTY_BLOCK:
          SetParameter(propertyBlock, parameterId, value);
          break;
        case MaterialPropertyTarget.GLOBAL:
          SetParameter(parameterId, value);
          break;
      }
    }

    public static void SetParameter(Material material, int parameterId, Color value) {
      if (!Utilities.IsValid(material)) {
        return;
      }

      material.SetColor(parameterId, value);
    }

    public static void SetParameter(MaterialPropertyBlock propertyBlock, int parameterId, Color value) {
      if (!Utilities.IsValid(propertyBlock)) {
        return;
      }

      propertyBlock.SetColor(parameterId, value);
    }

    public static void SetParameter(int parameterId, Color value) { VRCShader.SetGlobalColor(parameterId, value); }

    public static void SetParameter(
      MaterialPropertyTarget type,
      Material material,
      MaterialPropertyBlock propertyBlock,
      int parameterId,
      float value) {
      if (!IsValidParameterId(parameterId)) {
        return;
      }

      switch (type) {
        case MaterialPropertyTarget.MATERIAL:
          SetParameter(material, parameterId, value);
          break;
        case MaterialPropertyTarget.PROPERTY_BLOCK:
          SetParameter(propertyBlock, parameterId, value);
          break;
        case MaterialPropertyTarget.GLOBAL:
          SetParameter(parameterId, value);
          break;
      }
    }

    public static void SetParameter(Material material, int parameterId, float value) {
      if (!Utilities.IsValid(material)) {
        return;
      }

      material.SetFloat(parameterId, value);
    }

    public static void SetParameter(MaterialPropertyBlock propertyBlock, int parameterId, float value) {
      if (!Utilities.IsValid(propertyBlock)) {
        return;
      }

      propertyBlock.SetFloat(parameterId, value);
    }

    public static void SetParameter(int parameterId, float value) { VRCShader.SetGlobalFloat(parameterId, value); }

    public static void SetParameter(
      MaterialPropertyTarget type,
      Material material,
      MaterialPropertyBlock propertyBlock,
      int parameterId,
      int value) {
      if (!IsValidParameterId(parameterId)) {
        return;
      }

      switch (type) {
        case MaterialPropertyTarget.MATERIAL:
          SetParameter(material, parameterId, value);
          break;
        case MaterialPropertyTarget.PROPERTY_BLOCK:
          SetParameter(propertyBlock, parameterId, value);
          break;
        case MaterialPropertyTarget.GLOBAL:
          SetParameter(parameterId, value);
          break;
      }
    }

    public static void SetParameter(Material material, int parameterId, int value) {
      if (!Utilities.IsValid(material)) {
        return;
      }

      material.SetInt(parameterId, value);
    }

    public static void SetParameter(MaterialPropertyBlock propertyBlock, int parameterId, int value) {
      if (!Utilities.IsValid(propertyBlock)) {
        return;
      }

      propertyBlock.SetInt(parameterId, value);
    }

    public static void SetParameter(int parameterId, int value) { VRCShader.SetGlobalInteger(parameterId, value); }

    public static void SetParameter(
      MaterialPropertyTarget type,
      Material material,
      MaterialPropertyBlock propertyBlock,
      int parameterId,
      Vector3 value) {
      if (!IsValidParameterId(parameterId)) {
        return;
      }

      switch (type) {
        case MaterialPropertyTarget.MATERIAL:
          SetParameter(material, parameterId, value);
          break;
        case MaterialPropertyTarget.PROPERTY_BLOCK:
          SetParameter(propertyBlock, parameterId, value);
          break;
        case MaterialPropertyTarget.GLOBAL:
          SetParameter(parameterId, value);
          break;
      }
    }

    public static void SetParameter(Material material, int parameterId, Vector3 value) {
      if (!Utilities.IsValid(material)) {
        return;
      }

      material.SetVector(parameterId, value);
    }

    public static void SetParameter(MaterialPropertyBlock propertyBlock, int parameterId, Vector3 value) {
      if (!Utilities.IsValid(propertyBlock)) {
        return;
      }

      propertyBlock.SetVector(parameterId, value);
    }

    public static void SetParameter(int parameterId, Vector3 value) { VRCShader.SetGlobalVector(parameterId, value); }
  }
}
