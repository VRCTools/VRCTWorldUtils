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
using VRCTools.Event;
using VRCTools.World.LocalValues;

namespace VRCTools.World.Timer {
  /// <summary>
  /// Counts down to zero from a given or pre-defined start value.
  /// </summary>
  [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
  [AddComponentMenu("Timer/Local Timer")]
  public class LocalTimer : AbstractEventEmitter {
    
    /// <summary>
    /// Signifies that the timer has started counting down (resulting in a state transition to
    /// <see cref="TimerState.Running"/>).
    /// </summary>
    public const int EVENT_START = TimerEvents.STANDARD_EVENT_START;
    
    /// <summary>
    /// Signifies that the timer has been paused and will not proceed until resumed (resulting in a state transition
    /// to <see cref="TimerState.Paused"/>).
    /// </summary>
    public const int EVENT_PAUSE = TimerEvents.STANDARD_EVENT_PAUSE;
    
    /// <summary>
    /// Signifies that the timer has been resumed and will proceed counting down from its prior position (resulting in
    /// a state transition to <see cref="TimerState.Running"/>).
    /// </summary>
    public const int EVENT_RESUME = TimerEvents.STANDARD_EVENT_RESUME;
    
    /// <summary>
    /// Signifies that a timer has finished (resulting in a state transition to either
    /// <see cref="TimerState.Running"/> or <see cref="TimerState.Stopped"/> depending on whether this timer
    /// is recurring or not).
    /// </summary>
    public const int EVENT_FINISH = TimerEvents.STANDARD_EVENT_FINISH;
    
    /// <summary>
    /// Signifies that the timer has stopped (resulting in a state transition to <see cref="TimerState.Stopped"/>).
    /// </summary>
    public const int EVENT_STOP = TimerEvents.STANDARD_EVENT_STOPPED;
    
    /// <summary>
    /// Signifies that the timer has been reset.
    ///
    /// Resets do not transition the state of a timer on their own. They merely indicate that the timer has been reset
    /// back to the beginning.
    /// </summary>
    public const int EVENT_RESET = TimerEvents.STANDARD_EVENT_RESET;

    public const int EVENT_COUNT = TimerEvents.STANDARD_EVENT_COUNT;

    /// <summary>
    /// Defines whether the timer will automatically start when the component becomes active for the first time.
    /// </summary>
    public bool activateOnStart;
    
    /// <summary>
    /// Defines whether the timer shall automatically reset back to zero and count down again when it reaches its
    /// end.
    /// </summary>
    public bool recurring;
    
    /// <summary>
    /// Defines the duration with which this timer will be initialized if none is explicitly given.
    /// </summary>
    [Min(float.Epsilon)]
    public float defaultDuration = 10;

    /// <summary>
    /// Selects a value objects to which the remaining duration is written.
    ///
    /// Users should ensure that the default value of this float remains zero as startup logic may otherwise cause the
    /// values to desynchronize in some cases.
    /// </summary>
    public LocalFloat targetValue;

    /// <summary>
    /// Identifies the current state the timer is in.
    /// </summary>
    public TimerState State { get; private set; }
    
    /// <summary>
    /// Identifies the time remaining within the timer (in seconds).
    /// </summary>
    public float RemainingTime
    {
      get => this._remainingTime;
      private set
      {
        this._remainingTime = value;

        if (Utilities.IsValid(this.targetValue))
        {
          this.targetValue.State = Mathf.Max(0, value);
        }
      }
    }

    /// <summary>
    /// Checks whether the timer is currently stopped.
    /// </summary>
    public bool IsStopped => this.State == TimerState.Stopped;

    /// <summary>
    /// Checks whether the timer is currently running.
    /// </summary>
    public bool IsRunning => this.State == TimerState.Running;

    /// <summary>
    /// Checks whether the timer is currently paused.
    /// </summary>
    public bool IsPaused => this.State == TimerState.Paused;

    public override int EventCount => EVENT_COUNT;

    private float _remainingTime = 0f;
    private float _latestDuration;

    private void Start() {
      if (!this.activateOnStart)
      {
        // since this timer is not immediately activated on start, the target value needs to be populated with the
        // default of zero to ensure it remains in sync
        if (Utilities.IsValid(this.targetValue))
        {
          this.targetValue.State = 0f;
        }
        
        return;
      }
      
      this._Start();
    }

    private void Update() {
      if (!this.IsRunning) return;

      this.RemainingTime -= Time.deltaTime;
      if (this.RemainingTime > 0)
      {
        return;
      }
      
      if (this.recurring)
      {
        // when this timer is set to be recurring, we'll simply reset the internal time field back to the last
        // used duration value and start over again
        this.RemainingTime += this._latestDuration;
      }
      else
      {
        // when the timer is not set to be recurring, we'll set it back to idle and ensure the remaining time field
        // reads zero in case someone wants to display it
        this.State = TimerState.Stopped;
        this.RemainingTime = 0f;
      }
        
      // either way listeners will need to be notified that a cycle has finished - it is up to listeners to check the
      // recurring option if this matters to their respective logic
      this._EmitEvent(EVENT_FINISH);
    }

    /// <summary>
    /// Starts a timer with the given duration.
    ///
    /// When a timer is already active, it will be reset and the associated events will be invoked.
    /// </summary>
    /// <param name="duration">a timer duration</param>
    public void _Start(float duration) {
      if (duration <= 0) {
        Debug.LogWarning("[Local Timer] Zero or negative timer duration given - Ignored", this);
        return;
      }
      
      var previousState = this.State;

      // as part of the starting process, we also make sure to note the latest selected duration as this is relevant
      // for recurring timers where the next "iteration" of the timer will make use of the same duration
      this.State = TimerState.Running;
      this.RemainingTime = this._latestDuration = duration;

      switch (previousState)
      {
        case TimerState.Stopped:
          // if the timer was previously stopped, we simply indicate a regular start
          this._EmitEvent(EVENT_START);
          break;
        case TimerState.Paused:
          // if the timer was previously pause, we indicate a reset immediately followed by resume to mimic the
          // behaviour provided by _Reset
          this._EmitEvent(EVENT_RESET);
          this._EmitEvent(EVENT_RESUME);
          break;
        case TimerState.Running:
          // if the timer was already running, we simply indicate a reset to mimic the behaviour provided by _Reset
          this._EmitEvent(EVENT_RESET);
          break;
        default:
          // we have no clue what this means, log it as a bug and emit start for now - if you're ending up on this line
          // it probably means that you added a new state in TimerState and forgot to account for it ... idk why
          // you would ever want to do this, but you do you!
          Debug.LogError(
            $"[Local Timer] Unhandled state encountered in _Start - This is a bug (reported {previousState})", this);
          this._EmitEvent(EVENT_START);
          break;
      }
    }

    /// <summary>
    /// Starts a timer with the default duration.
    ///
    /// When a timer is already active, it will be reset and the associated events will be invoked.
    /// </summary>
    public void _Start() { this._Start(this.defaultDuration); }

    /// <summary>
    /// Pauses a currently running timer temporarily (leaving its remaining time untouched).
    ///
    /// When this method is invoked on a timer that has been stopped or paused, a warning will be emitted. Callers
    /// are expected to check before invoking these methods.
    /// </summary>
    public void _Pause() {
      if (!this.IsRunning) {
        Debug.LogWarning("[Local Timer] Attempted to pause stopped or already paused timer - Ignored", this);
        return;
      }

      this.State = TimerState.Paused;
      this._EmitEvent(EVENT_PAUSE);
    }

    /// <summary>
    /// Resumes a previously paused timer (resuming it from its previously remaining time).
    ///
    /// When this method is invoked on a timer that is not currently paused, a warning will be emitted. Callers are
    /// expected to check before invoking these methods.
    /// </summary>
    public void _Resume() {
      if (!this.IsPaused) {
        Debug.LogWarning("[Local Timer] Attempted to resume active or stopped timer - Ignored", this);
        return;
      }

      this.State = TimerState.Running;
      this._EmitEvent(EVENT_RESUME);
    }

    /// <summary>
    /// Stops a running or paused timer.
    ///
    /// Different from <see cref="_Pause"/>, timers which have been stopped cannot simply resume from their previous
    /// remaining timer duration. They may be restarted via one of the <see cref="_Start"/> overloads resulting in a
    /// reset back to <see cref="defaultDuration"/> or the given duration depending on the chosen overload.
    /// </summary>
    public void _Stop() {
      if (this.IsStopped) {
        Debug.LogWarning("[Local Timer] Attempted to stop already stopped timer - Ignored", this);
        return;
      }

      this.State = TimerState.Stopped;
      this.RemainingTime = 0f;
      this._EmitEvent(EVENT_STOP);
    }

    /// <summary>
    /// Resets a timer.
    ///
    /// If invoked on a timer that is currently running, a reset causes its duration to reset to the duration it was
    /// started with.
    ///
    /// If invoked on a paused timer, a reset causes it to reset to its initial duration but remain paused.
    ///
    /// If invoked on a stopped timer, this method basically acts as a noop. No events will be emitted.
    /// </summary>
    public void _Reset() {
      // if the timer is stopped, there is nothing to do here - the state will remain as is and nothing is modified
      if (this.State == TimerState.Stopped)
      {
        return;
      }
      
      // we reset the timer back to its initial duration regardless of the current state meaning:
      //
      //  - if the timer is currently running, it will keep running but reset back to the start again
      //  - if the timer is currently paused, it will remain paused but reset back to the start again
      //
      // meaning that callers need to explicitly decide whether they wish to resume a timer when invoking this
      // method
      this.RemainingTime = this._latestDuration;
      
      // regardless of which change we perform, we want to emit a RESET event to notify listeners that something out of
      // the ordinary has just happened, and they'll need to verify their internal copy of our state
      this._EmitEvent(EVENT_RESET);
    }
  }
}
