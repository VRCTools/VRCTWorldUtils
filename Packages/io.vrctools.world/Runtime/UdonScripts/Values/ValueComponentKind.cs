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

namespace VRCTools.World.Values
{
  /// <summary>
  /// Describes the rough category in which a given component falls.
  /// </summary>
  public enum ValueComponentKind
  {

    /// <summary>
    /// Describes a generic component which fits none of the other categories (this is the default if none was
    /// specified).
    /// </summary>
    Generic,

    /// <summary>
    /// Describes a component which supplies a value from an external source.
    /// </summary>
    Source,

    /// <summary>
    /// Describes a component which supplies a static value.
    /// </summary>
    Preset,

    /// <summary>
    /// Describes a component which stores a locally scoped value.
    /// </summary>
    LocalValue,

    /// <summary>
    /// Describes a component which stores a persisted value.
    /// </summary>
    PersistedValue,

    /// <summary>
    /// Describes a component which stores a synchronized value.
    /// </summary>
    SynchronizedValue,

    /// <summary>
    /// Describes a component which converts a value from one data type, range or meaning to another.
    /// </summary>
    Converter,

    /// <summary>
    /// Describes a component which exports a value to an external target.
    /// </summary>
    Applicator,

    /// <summary>
    /// Describes a component which displays or permits changing of a value within a user interface.
    ///
    /// This kind is a specialized combination of <see cref="Applicator"/> and/or <see cref="Source"/> and is thus
    /// separated out.
    /// </summary>
    UI
  }
}
