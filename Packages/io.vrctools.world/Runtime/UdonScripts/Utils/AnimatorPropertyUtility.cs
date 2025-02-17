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

using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VRCTools.World.Utils {
  public static class AnimatorPropertyUtility {
    public static int[] ResolveParameterIds(string[] parameterNames) {
      var parameterIds = new int[parameterNames.Length];

      for (var i = 0; i < parameterNames.Length; i++) {
        var parameterName = parameterNames[i];
        if (string.IsNullOrEmpty(parameterName)) {
          parameterIds[i] = -1;
          continue;
        }

        parameterIds[i] = Animator.StringToHash(parameterName);
      }

      return parameterIds;
    }
  }
}
