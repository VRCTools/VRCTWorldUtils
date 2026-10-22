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
using VRCTools.Common.Utils;

namespace VRCTools.World.LocalValues.Sources
{
  /// <summary>
  /// Checks whether the player has a certain set of permissions within the instance and writes true/false to a given
  /// output.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Sources/Instance Player Permission Source")]
  public class InstancePlayerPermissionSource : UdonSharpBehaviour
  {
    public LocalBoolean localOutput;

    public InstancePlayerPermissionType permissionType = InstancePlayerPermissionType.Owner;

    private void Start()
    {
      if (!Utilities.IsValid(this.localOutput))
      {
        UdonDebug.Error(this, "Output value is invalid - Disabled");
        this.enabled = false;
      }
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
      this._ComputeValue();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
      this._ComputeValue();
    }

    private void _ComputeValue()
    {
      var localPlayer = Networking.LocalPlayer;

      bool value;
      switch (this.permissionType)
      {
        case InstancePlayerPermissionType.Owner:
          value = localPlayer.isInstanceOwner;
          break;
        case InstancePlayerPermissionType.Master:
          value = localPlayer.isMaster;
          break;
        default:
          UdonDebug.Error(this, $"Unsupported player type {this.permissionType}");
          return;
      }

      this.localOutput.State = value;
    }
  }
}
