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

namespace VRCTools.World.LocalValues.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [RequireComponent(typeof(RawImage))]
  [AddComponentMenu("Local Values/Applicators/Local Raw Image Texture2D Applicator")]
  public class LocalRawImageTexture2DApplicator : UdonSharpBehaviour {
    public LocalTexture2D localValue;

    private RawImage _image;

    private void Start() {
      if (!Utilities.IsValid(this.localValue)) {
        Debug.LogError("[Raw Image Texture2D Applicator] Invalid local value reference - Disabled", this);
        this.enabled = false;
        return;
      }

      this._image = this.GetComponent<RawImage>();
      
      this.localValue._RegisterHandler(LocalTexture2D.EVENT_STATE_UPDATED, this, nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }

    public void _OnStateUpdated() {
      this._image.texture = this.localValue.State;
    }
  }
}
