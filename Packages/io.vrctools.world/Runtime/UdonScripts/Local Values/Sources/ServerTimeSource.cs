// Copyright 2026 .start <https://dotstart.tv>
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
using VRC.Udon;
using VRCTools.Common.Utils;

namespace VRCTools.World.LocalValues.Sources
{
  /// <summary>
  /// Dumps <see cref="Networking.GetServerTimeInSeconds()"/> into a value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Sources/Server Time Source")]
  public class ServerTimeSource : UdonSharpBehaviour
  {
    public LocalFloat localOutput;

    private void Start()
    {
      if (!Utilities.IsValid(this.localOutput))
      {
        UdonDebug.Error(this, "Output is invalid - Disabled");
        this.enabled = false;
        return;
      }
    }

    private void Update()
    {
      // realistically this probably should never overflow ... if it does, your players should consider finding
      // themselves something to do with their life
      this.localOutput.State = (float) Networking.GetServerTimeInSeconds();
    }
  }
}
