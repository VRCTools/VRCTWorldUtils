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
using VRC.SDKBase;
using VRCTools.Event;
using VRCTools.World.Values;

namespace VRCTools.World.SynchronizedValues {
  /// <summary>
  ///   Encapsulates a synchronized url value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Synchronized Values/Synchronized URL")]
  public class SynchronizedUrl : AbstractSynchronizedEventEmitter {
    public const int EVENT_STATE_UPDATED = ValueEvents.STANDARD_STATE_UPDATED;
    public const int EVENT_STATE_RESET = ValueEvents.STANDARD_STATE_RESET;
    public const int EVENT_COUNT = ValueEvents.STANDARD_COUNT;

    /// <summary>
    ///   Identifies the value with which this synchronized url will be initialized.
    /// </summary>
    public VRCUrl defaultValue = VRCUrl.Empty;

    /// <summary>
    ///   Stores the previously known state.
    /// </summary>
    private VRCUrl _previousState;

    /// <summary>
    ///   Stores the actual synchronized state.
    /// </summary>
    [UdonSynced]
    private VRCUrl state = VRCUrl.Empty;

    /// <summary>
    ///   Stores a counter which tracks the number of resets performed on this value.
    /// </summary>
    [UdonSynced]
    private int resetCounter;

    public override int EventCount => EVENT_COUNT;

    private int _previousResetCounterValue;

    /// <summary>
    ///   Retrieves or updates the synchronized state.
    ///   When setting the value, synchronization and event emission will only take place if the new value differs from
    ///   the current value. Otherwise, the operation effectively acts as a noop.
    /// </summary>
    public VRCUrl State {
      get => this.state;
      set {
        if (this.state.Equals(value)) return;

        if (!Networking.IsOwner(this.gameObject) && !this.StateInitialized)
        {
          Debug.LogWarning("[Synchronized URL] Attempted state update prior to initialization - Ignored", this);
          return;
        }

        this._EnsureOwnership();

        if (value == null)
          this.state = this._previousState = VRCUrl.Empty;
        else
          this.state = this._previousState = value;

        this._TrySynchronize();
        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    protected override void Start() {
      if (this.defaultValue == null)
        this.state = this._previousState = VRCUrl.Empty;
      else
        this.state = this._previousState = this.defaultValue;

      base.Start();
    }

    protected override void OnStateInitialized()
    {
      base.OnStateInitialized();

      this._EmitEvent(EVENT_STATE_UPDATED);
    }

    /// <summary>
    ///   Reverts the current value back to the default value.
    /// </summary>
    public void _Reset()
    {
      // ignore if the current state already reflects defaults as we do not wish to notify anyone in this case
      if ((this.state == null && this.defaultValue == null) || (this.state != null && this.state.Equals(this.defaultValue)))
      {
        return;
      }

      if (!Networking.IsOwner(this.gameObject) && !this.StateInitialized)
      {
        Debug.LogWarning("[Synchronized URL] Attempted state reset prior to initialization - Ignored", this);
        return;
      }
      
      this._EnsureOwnership();
      
      this._previousResetCounterValue = ++this.resetCounter;
      this.State = this.defaultValue;

      this._EmitEvent(EVENT_STATE_RESET);
    }

    public void _Clear() { this.State = VRCUrl.Empty; }

    public override void OnDeserialization() {
      var updated = !this._previousState.Equals(this.state) && this.StateInitialized;
      this._previousState = this.state;

      if (updated) this._EmitEvent(EVENT_STATE_UPDATED);
      
      var reset = this._previousResetCounterValue != this.resetCounter && this.StateInitialized;
      this._previousResetCounterValue = this.resetCounter;
      
      if (reset) this._EmitEvent(EVENT_STATE_RESET);

      // base has to be invoked last here as it also takes care of flagging the object as initialized thus requiring
      // the value logic to run first
      base.OnDeserialization();
    }
  }
}
