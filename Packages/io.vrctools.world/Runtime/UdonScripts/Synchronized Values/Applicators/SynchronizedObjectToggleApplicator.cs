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

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRCTools.World.Abstractions;

namespace VRCTools.World.SynchronizedValues.Applicators {
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Synchronized Values/Applicators/Synchronized Object Toggle Applicator")]
  public class SynchronizedObjectToggleApplicator : AbstractObjectToggleApplicator {
    public SynchronizedBoolean synchronizedValue;

    protected override bool State => this.synchronizedValue.State;

    private void Start() {
      if (!Utilities.IsValid(this.synchronizedValue)) {
        Debug.LogError("[Object Toggle Applicator] Synchronized value is invalid - Disabled", this);
        this.enabled = false;
        return;
      }

      this.synchronizedValue._RegisterHandler(SynchronizedBoolean.EVENT_STATE_UPDATED, this,
        nameof(this._OnStateUpdated));
      this._OnStateUpdated();
    }
  }
}
