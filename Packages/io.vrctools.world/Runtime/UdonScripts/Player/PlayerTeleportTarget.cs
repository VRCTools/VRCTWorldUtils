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
using UnityEditor;
using UnityEngine;
using VRC.SDKBase;

namespace VRCTools.World.Player {
  /// <summary>
  /// Provides a teleport target to which a local player may be teleported when triggered.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Player/Teleport Target")]
  public class PlayerTeleportTarget : UdonSharpBehaviour {
    public Transform target;

    public void _TeleportTo() {
      var localPlayer = Networking.LocalPlayer;
      var t = this.target;
      if (!Utilities.IsValid(t)) {
        t = this.transform;
      }

      localPlayer.TeleportTo(t.position, t.rotation);
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    private void OnDrawGizmos() {
      var t = this.target;
      if (!Utilities.IsValid(t)) {
        t = this.transform;
      }

      Handles.color = Color.blue;
      Handles.DrawWireDisc(t.position, t.up, .25f);

      Gizmos.color = Color.green;
      Gizmos.DrawLine(t.position, t.position + t.forward * .5f);
    }
    #endif
  }
}
