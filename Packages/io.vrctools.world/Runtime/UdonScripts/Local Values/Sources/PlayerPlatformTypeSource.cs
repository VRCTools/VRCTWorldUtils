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
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;

namespace VRCTools.World.LocalValues.Sources
{
  /// <summary>
  /// Checks whether the player is in VR or on desktop
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Sources/Player Platform Source")]
  public class PlayerPlatformTypeSource : UdonSharpBehaviour
  {
    public LocalBoolean localOutput;

    public PlayerPlatformType platformType = PlayerPlatformType.VR;

    private void Start()
    {
      if (!Utilities.IsValid(this.localOutput))
      {
        UdonDebug.Error(this, "Output is invalid - Disabled");
        this.enabled = false;
        return;
      }

      this._ComputeValue();
    }

    [CustomEvent]
    public void _ComputeValue()
    {
      var player = Networking.LocalPlayer;

      var result = player.IsUserInVR();
      if (this.platformType == PlayerPlatformType.Desktop)
      {
        result = !result;
      }

      this.localOutput.State = result;
    }
  }
}
