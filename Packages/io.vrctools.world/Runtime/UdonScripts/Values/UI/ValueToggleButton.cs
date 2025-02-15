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
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.UI {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [RequireComponent(typeof(Image))]
  [AddComponentMenu("Values/UI/Value Toggle Button")]
  public class ValueToggleButton : UdonSharpBehaviour {
    public ValueType source;
    public LocalBoolean localValue;
    public SynchronizedBoolean synchronizedValue;

    public Color activeColor = Color.white;
    public Color inactiveColor = Color.gray;

    public bool invert;

    private Image _image;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localValue, this.synchronizedValue)) {
        Debug.LogError("[Value Toggle Button] Boolean value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._image = this.GetComponent<Image>();

      ValueUtility.RegisterUpdateHandler(this.source, this.localValue, this.synchronizedValue, this,
        nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (Utilities.IsValid(this.localValue)) this.localValue._UnregisterHandler(this);

      if (Utilities.IsValid(this.synchronizedValue)) this.synchronizedValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      var state = ValueUtility.GetValue(this.source, this.localValue, this.synchronizedValue);
      this._image.color = state != this.invert ? this.activeColor : this.inactiveColor;
    }

    public void _Toggle() { ValueUtility.ToggleValue(this.source, this.localValue, this.synchronizedValue); }
  }
}
