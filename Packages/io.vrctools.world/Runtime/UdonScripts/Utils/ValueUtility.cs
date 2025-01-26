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

namespace VRCTools.World.Utils {
  /// <summary>
  ///   Provides various utility functions for dealing with various value types.
  /// </summary>
  public static class ValueUtility {
    /// <summary>
    ///   Remaps a given value from an original range to a given target range.
    /// </summary>
    /// <param name="value">an arbitrary value</param>
    /// <param name="from1">the lower bound of the original range</param>
    /// <param name="to1">the upper bound of the original range</param>
    /// <param name="from2">the lower bound of the target range</param>
    /// <param name="to2">the upper bound of the target range</param>
    /// <returns>a remapped value</returns>
    public static float Remap(float value, float from1, float to1, float from2, float to2) {
      return from2 + (value - from1) * (to2 - from1) / (to1 - from1);
    }

    /// <summary>
    ///   Remaps a given value from an original range to a given target range.
    /// </summary>
    /// <param name="value">an arbitrary value</param>
    /// <param name="from1">the lower bound of the original range</param>
    /// <param name="to1">the upper bound of the original range</param>
    /// <param name="from2">the lower bound of the target range</param>
    /// <param name="to2">the upper bound of the target range</param>
    /// <returns>a remapped value</returns>
    public static int Remap(int value, int from1, int to1, int from2, int to2) {
      return Mathf.RoundToInt(Remap((float)value, from1, to1, from2, to2));
    }
  }
}
