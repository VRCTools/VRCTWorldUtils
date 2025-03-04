﻿// Copyright 2025 .start <https://dotstart.tv>
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
  [RequireComponent(typeof(TMP_InputField))]
  [AddComponentMenu("Values/UI/Value Input Field")]
  public class ValueInputField : UdonSharpBehaviour {
    public ValueType source;
    public LocalString localValue;
    public SynchronizedString synchronizedValue;

    private TMP_InputField _inputField;
    private bool _updating;

    private void Start() {
      if (!ValueUtility.IsValid(this.source, this.localValue, this.synchronizedValue)) {
        Debug.LogError("[Input Field] String value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this._inputField = this.GetComponent<TMP_InputField>();

      ValueUtility.RegisterUpdateHandler(this.source, this.localValue, this.synchronizedValue, this,
        nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    private void OnDestroy() {
      if (Utilities.IsValid(this.localValue)) this.localValue._UnregisterHandler(this);

      if (Utilities.IsValid(this.synchronizedValue)) this.synchronizedValue._UnregisterHandler(this);
    }

    public void _OnStateUpdated() {
      this._updating = true;
      {
        this._inputField.text = ValueUtility.GetValue(this.source, this.localValue, this.synchronizedValue);
      }
      this._updating = false;
    }

    public void _OnValueChanged() {
      if (this._updating) return;

      ValueUtility.SetValue(this.source, this.localValue, this.synchronizedValue, this._inputField.text);
    }
  }
}
