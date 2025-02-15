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
using UnityEngine.Serialization;
using VRC.SDKBase;
using VRCTools.Event;
using VRCTools.World.Utils;

namespace VRCTools.World.Player {
  /// <summary>
  /// Provides a generic trigger volume capable of invoking another script when a player enters (or leaves) a given
  /// volume.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [RequireComponent(typeof(Collider))]
  [AddComponentMenu("Player/Trigger Volume")]
  public class PlayerTriggerVolume : AbstractEventEmitter {
    public const int EVENT_PLAYER_ENTER = 0;
    public const int EVENT_PLAYER_EXIT = 1;
    public const int EVENT_COUNT = 2;

    public UdonSharpBehaviour[] enterBehaviours;
    public string[] enterBehaviorEventNames;

    public UdonSharpBehaviour[] exitBehaviours;
    public string[] exitBehaviorEventNames;

    [PlayerType]
    public int acceptedPlayerTypes = PlayerType.ALL;

    public override int EventCount => EVENT_COUNT;

    /// <summary>
    /// Retrieves the last player which has interacted with this trigger volume.
    /// </summary>
    public VRCPlayerApi LastPlayer { get; private set; }

    private void Start() {
      this.enterBehaviours
        = MappingUtility.CheckCoherentMapping(this.enterBehaviours, this.enterBehaviorEventNames, this);
      this.exitBehaviours = MappingUtility.CheckCoherentMapping(this.exitBehaviours, this.exitBehaviorEventNames, this);
    }

    public override void OnPlayerTriggerEnter(VRCPlayerApi player) {
      var playerType = player.isLocal ? PlayerType.LOCAL : PlayerType.REMOTE;
      if (!PlayerType.HasFlag(playerType, this.acceptedPlayerTypes)) {
        return;
      }

      this.LastPlayer = player;

      for (var i = 0; i < this.enterBehaviours.Length; i++) {
        var behaviour = this.enterBehaviours[i];
        var eventName = this.enterBehaviorEventNames[i];
        if (!Utilities.IsValid(behaviour) || string.IsNullOrEmpty(eventName)) {
          continue;
        }

        behaviour.SendCustomEvent(eventName);
      }

      this._EmitEvent(EVENT_PLAYER_ENTER);
    }

    public override void OnPlayerTriggerExit(VRCPlayerApi player) {
      var playerType = player.isLocal ? PlayerType.LOCAL : PlayerType.REMOTE;
      if (!PlayerType.HasFlag(playerType, this.acceptedPlayerTypes)) {
        return;
      }

      this.LastPlayer = player;

      for (var i = 0; i < this.exitBehaviours.Length; i++) {
        var behaviour = this.exitBehaviours[i];
        var eventName = this.exitBehaviorEventNames[i];
        if (!Utilities.IsValid(behaviour) || string.IsNullOrEmpty(eventName)) {
          continue;
        }

        behaviour.SendCustomEvent(eventName);
      }

      this._EmitEvent(EVENT_PLAYER_EXIT);
    }
  }
}
