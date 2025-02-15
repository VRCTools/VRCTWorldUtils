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

namespace VRCTools.World.Player {
  public static class PlayerType {
    public const int NONE = 0;
    public const int LOCAL = 1;
    public const int REMOTE = 2;
    public const int ALL = 3;

    public static bool HasFlag(int value, int mask) {
      if (mask == ALL) return value != 0;

      if (mask == NONE) return value == 0;

      return (value & mask) != 0;
    }
  }
}
