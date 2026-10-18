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

namespace VRCTools.World.Values {
  public static class ValueEvents {
    #region Standard Values (local & synchronized)
    public const int STANDARD_STATE_UPDATED = 0;
    public const int STANDARD_STATE_RESET = 1;
    public const int STANDARD_COUNT = 2;
    #endregion

    #region Persisted Values
    public const int PERSISTED_LOADED = STANDARD_COUNT;
    public const int PERSISTED_PERSIST = STANDARD_COUNT + 1;
    public const int PERSISTED_COUNT = STANDARD_COUNT + 2;
    #endregion
  }
}
