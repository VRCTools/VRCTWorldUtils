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
  /// <summary>
  ///   Provides a set of utility functions for the purposes of managing 1:1 mappings of arrays within Udon scripts.
  /// </summary>
  internal static class MappingUtility {
    /// <summary>
    ///   Verifies that a set of associated arrays exhibit the same length.
    /// </summary>
    /// <param name="a">an array</param>
    /// <param name="b">an array</param>
    /// <param name="context">a Unity object to use as logging context</param>
    /// <typeparam name="A">the type of array a</typeparam>
    /// <typeparam name="B">the type of array B</typeparam>
    /// <returns>an adjusted copy of array a</returns>
    public static A[] CheckCoherentMapping<A, B>(A[] a, B[] b, Object context) {
      if (a.Length == b.Length) return a;

      if (a.Length < b.Length) {
        Debug.LogWarning(
          "Incoherent mapping detected - This likely means the component data has been corrupted - Additional parameter names will be ignored",
          context);
        return a;
      }

      Debug.LogWarning(
        "Incoherent mapping detected - This likely means the component data has been corrupted - Property list will be clamped",
        context);

      var copy = new A[b.Length];
      Array.Copy(a, copy, b.Length);
      return copy;
    }
  }
}
