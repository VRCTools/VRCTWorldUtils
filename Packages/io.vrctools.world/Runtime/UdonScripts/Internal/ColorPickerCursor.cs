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
using VRCTools.Event;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace VRCTools.World.Internal {
  /// <summary>
  /// Implements a simple cursor for use within color picker components.
  ///
  /// This script practically permits the placement of a 3D pickup on a 2D surface within the bounds of the color
  /// picker and will emit update events as the object is picked up and released in order to update the color picker
  /// with its new value.
  ///
  /// Note that the pickup on which this script is placed should not be synchronized. All synchronization is implemented
  /// through the actual color picker component.
  /// </summary>
  [RequireComponent(typeof(VRCPickup))]
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("")] // invisible as this component is internal
  public class ColorPickerCursor : AbstractEventEmitter {
    public const int EVENT_POSITION_UPDATED = 0;
    public const int EVENT_COUNT = 1;

    /// <summary>
    ///   Defines the actual size of the color picker UI.
    /// </summary>
    public float size = 90;

    /// <summary>
    ///   Defines the scale of the color picker UI.
    /// </summary>
    public float scale = 0.01f;

    /// <summary>
    ///   Specified the cursor which displays the actual picked color.
    /// </summary>
    public RawImage uiCursor;

    private bool _held;
    private Vector2 _maxPosition;

    private Vector2 _minPosition;
    private float _originalDepth;
    private VRCPickup _pickup;

    private Rigidbody _rigidbody;

    public override int EventCount => EVENT_COUNT;

    public Vector2 Position {
      get {
        var pos = new Vector2(this.transform.localPosition.x, this.transform.localPosition.y);
        pos /= this._maxPosition;
        pos += Vector2.one;
        pos *= .5f;
        pos.x = 1 - pos.x;
        return pos;
      }
      set {
        var pos = new Vector2(Mathf.Clamp01(value.x), Mathf.Clamp01(value.y));
        pos.x = 1 - pos.x;
        pos *= 2;
        pos -= Vector2.one;
        pos *= this._maxPosition;

        this.transform.localPosition = new Vector3(pos.x, pos.y, this._originalDepth);

        this._UpdateUI();
        this._EmitEvent(EVENT_POSITION_UPDATED);
      }
    }

    protected void Start() {
      if (!Utilities.IsValid(this.uiCursor)) {
        Debug.LogError("[ColorPicker] Invalid cursor reference - Disabled");
        this.enabled = false;
        return;
      }

      this._rigidbody = this.GetComponent<Rigidbody>();
      this._pickup = this.GetComponent<VRCPickup>();
      this._originalDepth = this.transform.localPosition.z;

      var bounds = this.size * this.scale * .5f;
      this._minPosition = new Vector2(-bounds, -bounds);
      this._maxPosition = new Vector2(bounds, bounds);
    }

    protected void Update() {
      if (!this._held) return;

      this._UpdateUI();
    }

    public override void OnPickup() { this._held = true; }

    private void _UpdateUI() {
      var localPos = this.transform.localPosition;

      var pos = new Vector2(localPos.x, localPos.y);
      pos.x = Mathf.Clamp(pos.x, this._minPosition.x, this._maxPosition.x);
      pos.y = Mathf.Clamp(pos.y, this._minPosition.y, this._maxPosition.y);
      pos /= this.scale;

      this.uiCursor.rectTransform.localPosition = new Vector3(pos.x, pos.y, 0);
    }

    public override void OnDrop() {
      this._held = false;

      var localPos = this.transform.localPosition;
      var pos = new Vector2(localPos.x, localPos.y);
      pos.x = Mathf.Clamp(pos.x, this._minPosition.x, this._maxPosition.x);
      pos.y = Mathf.Clamp(pos.y, this._minPosition.y, this._maxPosition.y);

      this.transform.localPosition = new Vector3(pos.x, pos.y, this._originalDepth);

      this._EmitEvent(EVENT_POSITION_UPDATED);
    }
  }
}
