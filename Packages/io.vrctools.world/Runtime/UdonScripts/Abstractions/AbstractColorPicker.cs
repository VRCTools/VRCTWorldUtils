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
using UnityEngine.UI;
using VRC.SDKBase;
using VRCTools.World.Internal;

namespace VRCTools.World.Abstractions {
  /// <summary>
  ///   Provides a basis for a generic color picker component.
  /// </summary>
  public abstract class AbstractColorPicker : UdonSharpBehaviour {
    public ColorPickerCursor cursor;
    public RawImage crosshair;

    protected virtual void Start() {
      if (!Utilities.IsValid(this.cursor)) {
        Debug.LogError("[Color Picker] Invalid cursor reference - Disabled");
        this.enabled = false;
        return;
      }

      this.cursor._RegisterHandler(ColorPickerCursor.EVENT_POSITION_UPDATED, this, nameof(this._OnPositionUpdated));
    }

    protected virtual void OnDestroy() {
      if (!Utilities.IsValid(this.cursor)) return;

      this.cursor._UnregisterHandler(this);
    }

    protected virtual void _UpdateColor(Color color) {
      if (Utilities.IsValid(this.crosshair)) this.crosshair.color = color;
    }

    public virtual void _OnPositionUpdated() {
      var pos = this.cursor.Position;
      this._UpdateColor(Color.HSVToRGB(pos.x, 1, pos.y));
    }
  }
}
