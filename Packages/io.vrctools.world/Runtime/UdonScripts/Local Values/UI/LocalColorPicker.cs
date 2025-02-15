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
using VRCTools.World.Abstractions;

namespace VRCTools.World.LocalValues.UI {
  /// <summary>
  ///   Provides a color picker component which may be paired with a <see cref="LocalColor" /> component in order
  ///   to permit dynamic color changes.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/UI/Local Color Picker")]
  public class LocalColorPicker : AbstractColorPicker {
    public LocalColor localValue;

    private bool _updating;

    protected override void Start() {
      if (!Utilities.IsValid(this.localValue)) {
        Debug.LogError("[Color Picker] Invalid local value reference - Disabled");
        this.enabled = false;
        return;
      }

      this.localValue._RegisterHandler(LocalColor.EVENT_STATE_UPDATED, this,
        nameof(this._OnStateUpdated));

      base.Start();
    }

    protected override void OnDestroy() {
      base.OnDestroy();

      if (!Utilities.IsValid(this.localValue)) return;

      this.localValue._UnregisterHandler(this);
    }

    protected override void _UpdateColor(Color color) {
      this.localValue.State = color;
      base._UpdateColor(color);
    }

    public override void _OnPositionUpdated() {
      if (this._updating) return;

      base._OnPositionUpdated();
    }

    public void _OnStateUpdated() {
      this._updating = true;
      {
        var color = this.localValue.State;

        // Note: UdonSharp does not understand "out _" syntax - we need this useless variable here
        Color.RGBToHSV(color, out var h, out var s, out var v);
        this.cursor.Position = new Vector2(h, v);
      }
      this._updating = false;
    }
  }
}
