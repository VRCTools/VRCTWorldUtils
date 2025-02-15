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

using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.UI {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [RequireComponent(typeof(TextMeshProUGUI))]
  [AddComponentMenu("Values/UI/String Label")]
  public class StringLabel : UdonSharpBehaviour {
    public ValueType source;
    public LocalString localValue;
    public SynchronizedString synchronizedValue;

    private string _template;
    private TextMeshProUGUI _text;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localValue, this.synchronizedValue)) {
        Debug.LogError("[String Label] String value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._text = this.GetComponent<TextMeshProUGUI>();
      this._template = this._text.text;

      ValueUtility.RegisterUpdateHandler(this.source, this.localValue, this.synchronizedValue, this,
        nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (Utilities.IsValid(this.localValue)) this.localValue._UnregisterHandler(this);

      if (Utilities.IsValid(this.synchronizedValue)) this.synchronizedValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      this._text.text = string.Format(this._template,
        ValueUtility.GetValue(this.source, this.localValue, this.synchronizedValue));
    }
  }
}
