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
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.UI {
  /// <summary>
  ///   Provides a color picker component which may be paired with a <see cref="LocalColor" /> component in order
  ///   to permit dynamic color changes.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/UI/Color Picker")]
  public class ColorPicker : UdonSharpBehaviour {
    public ValueType source;
    public LocalColor localValue;
    public SynchronizedColor synchronizedValue;

    public ColorPickerCursor cursor;
    public RawImage crosshair;

    private bool _updating;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localValue, this.synchronizedValue)) {
        Debug.LogError("[Color Picker] Invalid local value reference - Disabled");
        this.enabled = false;
        return;
      }

      if (!Utilities.IsValid(this.cursor)) {
        Debug.LogError("[Color Picker] Invalid cursor reference - Disabled");
        this.enabled = false;
        return;
      }

      ValueUtility.RegisterUpdateHandler(this.source, this.localValue, this.synchronizedValue, this,
        nameof(this._OnStateUpdated));

      this.cursor._RegisterHandler(ColorPickerCursor.EVENT_POSITION_UPDATED, this, nameof(this._OnPositionUpdated));
    }

    private void OnDestroy() {
      if (Utilities.IsValid(this.cursor)) this.cursor._UnregisterHandler(this);

      if (Utilities.IsValid(this.localValue)) this.localValue._UnregisterHandler(this);

      if (Utilities.IsValid(this.synchronizedValue)) this.synchronizedValue._UnregisterHandler(this);
    }

    private void _UpdateColor(Color color) {
      ValueUtility.SetValue(this.source, this.localValue, this.synchronizedValue, color);

      if (Utilities.IsValid(this.crosshair))
        this.crosshair.color = color;
    }

    public void _OnPositionUpdated() {
      if (this._updating) return;

      var pos = this.cursor.Position;
      this._UpdateColor(Color.HSVToRGB(pos.x, 1, pos.y));
    }

    public void _OnStateUpdated() {
      this._updating = true;
      {
        var color = ValueUtility.GetValue(this.source, this.localValue, this.synchronizedValue);

        // Note: UdonSharp does not understand "out _" syntax - we need this useless variable here
        Color.RGBToHSV(color, out var h, out var s, out var v);
        this.cursor.Position = new Vector2(h, v);
      }
      this._updating = false;
    }
  }
}
