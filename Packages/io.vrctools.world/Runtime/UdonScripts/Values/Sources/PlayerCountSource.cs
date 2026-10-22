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
using VRCTools.World.LocalValues;
using VRCTools.World.SynchronizedValues;
using VRCTools.World.Utils;

namespace VRCTools.World.Values.Sources
{
  /// <summary>
  /// Writes the current player count to a given output value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Values/Sources/Player Count Source")]
  public class PlayerCountSource : UdonSharpBehaviour
  {
    public ValueType output;
    public LocalInt localOutput;
    public SynchronizedInt synchronizedOutput;

    public PlayerCountMode mode = PlayerCountMode.Current;

    private void Start()
    {
      if (!ValueUtility.IsValid(this.output, this.localOutput, this.synchronizedOutput))
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
      if (this.mode == PlayerCountMode.TotalJoined)
      {
        return;
      }

      this._ComputeValue();
    }

    private void _ComputeValue()
    {
      // in all modes except current, the output depends on the current value - For synchronized values, we'll need to
      // check ownership before updating them (only the current owner sources values for synchronized targets)
      if (this.mode != PlayerCountMode.Current &&
          !ValueUtility.IsLocallyOwned(this.output, this.synchronizedOutput))
      {
        return;
      }

      var previousValue = ValueUtility.GetValue(this.output, this.localOutput, this.synchronizedOutput);

      int result;
      switch (this.mode)
      {
        case PlayerCountMode.Current:
          result = this._ComputeCurrent();
          break;
        case PlayerCountMode.Max:
          result = this._ComputeMax(previousValue);
          break;
        case PlayerCountMode.TotalJoined:
          result = this._ComputeTotalJoined(previousValue);
          break;
        default:
          UdonDebug.Error(this, "Unsupported mode - This is a bug");
          return;
      }

      ValueUtility.SetValue(this.output, this.localOutput, this.synchronizedOutput, result);
    }

    private int _ComputeCurrent()
    {
      return VRCPlayerApi.GetPlayerCount();
    }

    private int _ComputeMax(int previousValue)
    {
      var currentPlayerCount = VRCPlayerApi.GetPlayerCount();
      return Mathf.Max(currentPlayerCount, previousValue);
    }

    private int _ComputeTotalJoined(int previousValue)
    {
      return previousValue + 1;
    }
  }
}
