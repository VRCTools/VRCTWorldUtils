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
using VRCTools.Common.Attributes;
using VRCTools.Common.Utils;
using VRCTools.Event;
using VRCTools.World.Values;

namespace VRCTools.World.SynchronizedValues {
  /// <summary>
  ///   Encapsulates a synchronized boolean value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
  [AddComponentMenu("Synchronized Values/Synchronized Boolean")]
  public class SynchronizedBoolean : AbstractSynchronizedEventEmitter {
    public const int EVENT_STATE_UPDATED = ValueEvents.STANDARD_STATE_UPDATED;
    public const int EVENT_STATE_RESET = ValueEvents.STANDARD_STATE_RESET;
    public const int EVENT_COUNT = ValueEvents.STANDARD_COUNT;

    /// <summary>
    ///   Identifies the value with which this synchronized boolean will be initialized.
    /// </summary>
    public bool defaultValue;

    /// <summary>
    ///   Stores the previously known state.
    /// </summary>
    private bool _previousState;

    /// <summary>
    ///   Stores the actual synchronized state.
    /// </summary>
    [UdonSynced]
    private bool state;

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
    public bool State {
      get => this.state;
      set {
        if (this.state == value) return;

        if (!this.IsLocallyOwned() && !this.IsStateInitialized)
        {
          UdonDebug.Warn(this, "Attempted state update prior to initialization - Ignored");
          return;
        }

        this._EnsureOwnership();

        this.state = this._previousState = value;

        this._TrySynchronize();
        this._EmitEvent(EVENT_STATE_UPDATED);
      }
    }

    [MustCallBase]
    protected override void Start() {
      this.state = this._previousState = this.defaultValue;

      base.Start();
    }

    protected override void _OnStateInitialized()
    {
      this._EmitEvent(EVENT_STATE_UPDATED);
    }

    /// <summary>
    ///   Reverts the current value back to the default value.
    /// </summary>
    [CustomEvent]
    public void _Reset()
    {
      // ignore if the current state already reflects defaults as we do not wish to notify anyone in this case
      if (this.state == this.defaultValue)
      {
        return;
      }

      if (!this.IsLocallyOwned() && !this.IsStateInitialized)
      {
        UdonDebug.Warn(this, "Attempted state reset prior to initialization - Ignored");
        return;
      }
      
      this._EnsureOwnership();
      
      this._previousResetCounterValue = ++this.resetCounter;
      this.State = this.defaultValue;

      this._EmitEvent(EVENT_STATE_RESET);
    }

    /// <summary>
    ///   Toggles the current value of this synchronized value (e.g. when true, toggles to false; when false, toggles to
    ///   true).
    /// </summary>
    [CustomEvent]
    public void _Toggle() { this.State = !this.State; }

    /// <summary>
    ///   Sets the synchronized value to true.
    ///   When the synchronized value is already true, no synchronization or event emission takes place.
    /// </summary>
    [CustomEvent]
    public void _SetTrue() { this.State = true; }

    /// <summary>
    ///   Sets the synchronized value to false.
    ///   When the synchronized value is already false, no synchronization or event emission takes place.
    /// </summary>
    [CustomEvent]
    public void _SetFalse() { this.State = false; }

    [MustCallBase]
    public override void OnDeserialization() {
      var updated = this._previousState != this.state && this.IsStateInitialized;
      this._previousState = this.state;

      if (updated) this._EmitEvent(EVENT_STATE_UPDATED);

      var reset = this._previousResetCounterValue != this.resetCounter && this.IsStateInitialized;
      this._previousResetCounterValue = this.resetCounter;
      
      if (reset) this._EmitEvent(EVENT_STATE_RESET);

      // base has to be invoked last here as it also takes care of flagging the object as initialized thus requiring
      // the value logic to run first
      base.OnDeserialization();
    }
  }
}
