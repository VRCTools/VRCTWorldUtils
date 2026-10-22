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
using VRC.SDK3.Data;
using VRC.SDKBase;
using VRCTools.Common.Utils;

namespace VRCTools.World.LocalValues.Sources
{
  /// <summary>
  /// Counts the number of players within a given volume.
  /// </summary>
  [RequireComponent(typeof(Collider))]
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Local Values/Sources/Player Count Volume Source")]
  public class PlayerCountVolumeSource : UdonSharpBehaviour
  {
    public LocalInt localOutput;

    private DataList _playerList = new DataList();

    private void Start()
    {
      if (!Utilities.IsValid(this.localOutput))
      {
        UdonDebug.Error(this, "Output is invalid - Disabled");
        this.enabled = false;
        return;
      }

      this.localOutput.State = 0;
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player)
    {
      var playerId = new DataToken(player.playerId);
      if (this._playerList.Contains(playerId))
      {
        return;
      }

      this._playerList.Add(playerId);
      this._ComputeValue();
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player)
    {
      var playerId = new DataToken(player.playerId);
      if (!this._playerList.Contains(playerId))
      {
        return;
      }

      this._playerList.Remove(playerId);
      this._ComputeValue();
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
      var playerId = new DataToken(player.playerId);
      if (!this._playerList.Contains(playerId))
      {
        return;
      }

      this._playerList.Remove(playerId);
      this._ComputeValue();
    }

    private void _ComputeValue()
    {
      this.localOutput.State = this._playerList.Count;
    }
  }
}
