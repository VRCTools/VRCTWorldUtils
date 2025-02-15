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

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Values;
using ValueType = VRCTools.World.Values.ValueType;

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

    public static ValueSource ToSource(ValueType type) {
      switch (type) {
        case ValueType.LOCAL: return ValueSource.LOCAL;
        case ValueType.SYNCHRONIZED: return ValueSource.SYNCHRONIZED;
      }

      Debug.LogError($"Received unknown value type {type}");
      return ValueSource.STATIC;
    }

    public static bool IsValid(ValueSource type, object localValue, object synchronizedValue) {
      var valid = true;

      switch (type) {
        case ValueSource.LOCAL:
          valid = Utilities.IsValid(localValue);
          break;
        case ValueSource.SYNCHRONIZED:
          valid = Utilities.IsValid(synchronizedValue);
          break;
      }

      return valid;
    }

    public static bool IsValid(ValueType type, object localValue, object synchronizedValue) {
      return IsValid(ToSource(type), localValue, synchronizedValue);
    }

    public static void RegisterUpdateHandler(
      ValueSource source,
      LocalBoolean localValue,
      SynchronizedBoolean synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      switch (source) {
        case ValueSource.LOCAL:
          localValue._RegisterHandler(LocalBoolean.EVENT_STATE_UPDATED, handler, eventName);
          break;
        case ValueSource.SYNCHRONIZED:
          synchronizedValue._RegisterHandler(SynchronizedBoolean.EVENT_STATE_UPDATED, handler, eventName);
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueType type,
      LocalBoolean localValue,
      SynchronizedBoolean synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      RegisterUpdateHandler(ToSource(type), localValue, synchronizedValue, handler, eventName);
    }

    public static bool GetValue(
      ValueSource source,
      bool staticValue,
      LocalBoolean localValue,
      SynchronizedBoolean synchronizedValue) {
      var state = staticValue;

      switch (source) {
        case ValueSource.LOCAL:
          state = localValue.State;
          break;
        case ValueSource.SYNCHRONIZED:
          state = synchronizedValue.State;
          break;
      }

      return state;
    }

    public static bool GetValue(ValueType type, LocalBoolean localValue, SynchronizedBoolean synchronizedValue) {
      return GetValue(ToSource(type), false, localValue, synchronizedValue);
    }

    public static void SetValue(
      ValueType type,
      LocalBoolean localValue,
      SynchronizedBoolean synchronizedValue,
      bool value) {
      switch (type) {
        case ValueType.LOCAL:
          localValue.State = value;
          break;
        case ValueType.SYNCHRONIZED:
          synchronizedValue.State = value;
          break;
      }
    }

    public static void ToggleValue(ValueType type, LocalBoolean localValue, SynchronizedBoolean synchronizedValue) {
      switch (type) {
        case ValueType.LOCAL:
          localValue._Toggle();
          break;
        case ValueType.SYNCHRONIZED:
          synchronizedValue._Toggle();
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueSource source,
      LocalColor localValue,
      SynchronizedColor synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      switch (source) {
        case ValueSource.LOCAL:
          localValue._RegisterHandler(LocalColor.EVENT_STATE_UPDATED, handler, eventName);
          break;
        case ValueSource.SYNCHRONIZED:
          synchronizedValue._RegisterHandler(SynchronizedColor.EVENT_STATE_UPDATED, handler, eventName);
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueType type,
      LocalColor localValue,
      SynchronizedColor synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      RegisterUpdateHandler(ToSource(type), localValue, synchronizedValue, handler, eventName);
    }

    public static Color GetValue(
      ValueSource source,
      Color staticColor,
      LocalColor localValue,
      SynchronizedColor synchronizedValue) {
      var state = staticColor;

      switch (source) {
        case ValueSource.LOCAL:
          state = localValue.State;
          break;
        case ValueSource.SYNCHRONIZED:
          state = synchronizedValue.State;
          break;
      }

      return state;
    }

    public static Color GetValue(ValueType type, LocalColor localValue, SynchronizedColor synchronizedValue) {
      return GetValue(ToSource(type), Color.black, localValue, synchronizedValue);
    }

    public static void SetValue(
      ValueType type,
      LocalColor localValue,
      SynchronizedColor synchronizedValue,
      Color value) {
      switch (type) {
        case ValueType.LOCAL:
          localValue.State = value;
          break;
        case ValueType.SYNCHRONIZED:
          synchronizedValue.State = value;
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueSource source,
      LocalFloat localValue,
      SynchronizedFloat synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      switch (source) {
        case ValueSource.LOCAL:
          localValue._RegisterHandler(LocalFloat.EVENT_STATE_UPDATED, handler, eventName);
          break;
        case ValueSource.SYNCHRONIZED:
          synchronizedValue._RegisterHandler(SynchronizedFloat.EVENT_STATE_UPDATED, handler, eventName);
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueType type,
      LocalFloat localValue,
      SynchronizedFloat synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      RegisterUpdateHandler(ToSource(type), localValue, synchronizedValue, handler, eventName);
    }

    public static float GetValue(
      ValueSource source,
      float staticValue,
      LocalFloat localValue,
      SynchronizedFloat synchronizedValue) {
      var state = staticValue;

      switch (source) {
        case ValueSource.LOCAL:
          state = localValue.State;
          break;
        case ValueSource.SYNCHRONIZED:
          state = synchronizedValue.State;
          break;
      }

      return state;
    }

    public static float GetValue(ValueType type, LocalFloat localValue, SynchronizedFloat synchronizedValue) {
      return GetValue(ToSource(type), 0f, localValue, synchronizedValue);
    }

    public static void SetValue(
      ValueType type,
      LocalFloat localValue,
      SynchronizedFloat synchronizedValue,
      float value) {
      switch (type) {
        case ValueType.LOCAL:
          localValue.State = value;
          break;
        case ValueType.SYNCHRONIZED:
          synchronizedValue.State = value;
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueSource source,
      LocalInt localValue,
      SynchronizedInt synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      switch (source) {
        case ValueSource.LOCAL:
          localValue._RegisterHandler(LocalInt.EVENT_STATE_UPDATED, handler, eventName);
          break;
        case ValueSource.SYNCHRONIZED:
          synchronizedValue._RegisterHandler(SynchronizedInt.EVENT_STATE_UPDATED, handler, eventName);
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueType type,
      LocalInt localValue,
      SynchronizedInt synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      RegisterUpdateHandler(ToSource(type), localValue, synchronizedValue, handler, eventName);
    }

    public static int GetValue(
      ValueSource source,
      int staticValue,
      LocalInt localValue,
      SynchronizedInt synchronizedValue) {
      var state = staticValue;

      switch (source) {
        case ValueSource.LOCAL:
          state = localValue.State;
          break;
        case ValueSource.SYNCHRONIZED:
          state = synchronizedValue.State;
          break;
      }

      return state;
    }

    public static int GetValue(ValueType type, LocalInt localValue, SynchronizedInt synchronizedValue) {
      return GetValue(ToSource(type), 0, localValue, synchronizedValue);
    }

    public static void SetValue(ValueType type, LocalInt localValue, SynchronizedInt synchronizedValue, int value) {
      switch (type) {
        case ValueType.LOCAL:
          localValue.State = value;
          break;
        case ValueType.SYNCHRONIZED:
          synchronizedValue.State = value;
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueSource source,
      LocalString localValue,
      SynchronizedString synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      switch (source) {
        case ValueSource.LOCAL:
          localValue._RegisterHandler(LocalString.EVENT_STATE_UPDATED, handler, eventName);
          break;
        case ValueSource.SYNCHRONIZED:
          synchronizedValue._RegisterHandler(SynchronizedString.EVENT_STATE_UPDATED, handler, eventName);
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueType type,
      LocalString localValue,
      SynchronizedString synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      RegisterUpdateHandler(ToSource(type), localValue, synchronizedValue, handler, eventName);
    }

    public static string GetValue(
      ValueSource source,
      string staticValue,
      LocalString localValue,
      SynchronizedString synchronizedValue) {
      var state = staticValue;

      switch (source) {
        case ValueSource.LOCAL:
          state = localValue.State;
          break;
        case ValueSource.SYNCHRONIZED:
          state = synchronizedValue.State;
          break;
      }

      return state;
    }

    public static string GetValue(ValueType type, LocalString localValue, SynchronizedString synchronizedValue) {
      return GetValue(ToSource(type), string.Empty, localValue, synchronizedValue);
    }

    public static void SetValue(
      ValueType type,
      LocalString localValue,
      SynchronizedString synchronizedValue,
      string value) {
      switch (type) {
        case ValueType.LOCAL:
          localValue.State = value;
          break;
        case ValueType.SYNCHRONIZED:
          synchronizedValue.State = value;
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueSource source,
      LocalUrl localValue,
      SynchronizedUrl synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      switch (source) {
        case ValueSource.LOCAL:
          localValue._RegisterHandler(LocalUrl.EVENT_STATE_UPDATED, handler, eventName);
          break;
        case ValueSource.SYNCHRONIZED:
          synchronizedValue._RegisterHandler(SynchronizedUrl.EVENT_STATE_UPDATED, handler, eventName);
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueType type,
      LocalUrl localValue,
      SynchronizedUrl synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      RegisterUpdateHandler(ToSource(type), localValue, synchronizedValue, handler, eventName);
    }

    public static VRCUrl GetValue(
      ValueSource source,
      VRCUrl staticValue,
      LocalUrl localValue,
      SynchronizedUrl synchronizedValue) {
      var state = staticValue;

      switch (source) {
        case ValueSource.LOCAL:
          state = localValue.State;
          break;
        case ValueSource.SYNCHRONIZED:
          state = synchronizedValue.State;
          break;
      }

      return state;
    }

    public static VRCUrl GetValue(ValueType type, LocalUrl staticValue, SynchronizedUrl synchronizedValue) {
      return GetValue(ToSource(type), VRCUrl.Empty, staticValue, synchronizedValue);
    }

    public static void SetValue(
      ValueType type,
      LocalUrl staticValue,
      SynchronizedUrl synchronizedValue,
      VRCUrl value) {
      switch (type) {
        case ValueType.LOCAL:
          staticValue.State = value;
          break;
        case ValueType.SYNCHRONIZED:
          synchronizedValue.State = value;
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueSource source,
      LocalVector3 localValue,
      SynchronizedVector3 synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      switch (source) {
        case ValueSource.LOCAL:
          localValue._RegisterHandler(LocalVector3.EVENT_STATE_UPDATED, handler, eventName);
          break;
        case ValueSource.SYNCHRONIZED:
          synchronizedValue._RegisterHandler(SynchronizedVector3.EVENT_STATE_UPDATED, handler, eventName);
          break;
      }
    }

    public static void RegisterUpdateHandler(
      ValueType type,
      LocalVector3 localValue,
      SynchronizedVector3 synchronizedValue,
      UdonSharpBehaviour handler,
      string eventName) {
      RegisterUpdateHandler(ToSource(type), localValue, synchronizedValue, handler, eventName);
    }

    public static Vector3 GetValue(
      ValueSource source,
      Vector3 staticValue,
      LocalVector3 localValue,
      SynchronizedVector3 synchronizedValue) {
      var state = staticValue;

      switch (source) {
        case ValueSource.LOCAL:
          state = localValue.State;
          break;
        case ValueSource.SYNCHRONIZED:
          state = synchronizedValue.State;
          break;
      }

      return state;
    }

    public static Vector3 GetValue(ValueType type, LocalVector3 localValue, SynchronizedVector3 synchronizedValue) {
      return GetValue(ToSource(type), Vector3.zero, localValue, synchronizedValue);
    }

    public static void SetValue(ValueType type, LocalVector3 localValue, SynchronizedVector3 synchronizedValue) {
      switch (type) {
        case ValueType.LOCAL:
          localValue.State = synchronizedValue.State;
          break;
        case ValueType.SYNCHRONIZED:
          synchronizedValue.State = synchronizedValue.State;
          break;
      }
    }
  }
}
