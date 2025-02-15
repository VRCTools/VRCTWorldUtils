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

namespace VRCTools.World.SynchronizedValues.UI {
  /// <summary>
  ///   Provides an implementation of a toggle button using a synchronized boolean value.
  /// </summary>
  [RequireComponent(typeof(Image))]
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/UI/Synchronized Toggle Button")]
  public class SynchronizedToggleButton : UdonSharpBehaviour {
    public SynchronizedBoolean synchronizedValue;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    public bool invert;

    private Image _image;

    private void Start() {
      if (!Utilities.IsValid(this.synchronizedValue)) {
        Debug.LogError("[Synchronized Toggle Button] Synchronized value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._image = this.GetComponent<Image>();

      this.synchronizedValue._RegisterHandler(SynchronizedBoolean.EVENT_STATE_UPDATED, this,
        nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (!Utilities.IsValid(this.synchronizedValue)) return;

      this.synchronizedValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      this._image.color = this.synchronizedValue.State != this.invert ? this.activeColor : this.inactiveColor;
    }

    public void _Toggle() { this.synchronizedValue._Toggle(); }
  }
}
