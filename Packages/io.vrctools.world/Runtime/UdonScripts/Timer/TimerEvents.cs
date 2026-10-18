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

namespace VRCTools.World.Timer
{
    public static class TimerEvents
    {
        /// <summary>
        /// Signifies that the timer has started counting down (resulting in a state transition to
        /// <see cref="TimerState.Running"/>).
        /// </summary>
        public const int STANDARD_EVENT_START = 0;
    
        /// <summary>
        /// Signifies that the timer has been paused and will not proceed until resumed (resulting in a state transition
        /// to <see cref="TimerState.Paused"/>).
        /// </summary>
        public const int STANDARD_EVENT_PAUSE = 1;
    
        /// <summary>
        /// Signifies that the timer has been resumed and will proceed counting down from its prior position (resulting in
        /// a state transition to <see cref="TimerState.Running"/>).
        /// </summary>
        public const int STANDARD_EVENT_RESUME = 2;
    
        /// <summary>
        /// Signifies that a timer has finished (resulting in a state transition to either
        /// <see cref="TimerState.Running"/> or <see cref="TimerState.Stopped"/> depending on whether this timer
        /// is recurring or not).
        /// </summary>
        public const int STANDARD_EVENT_FINISH = 3;
    
        /// <summary>
        /// Signifies that the timer has stopped (resulting in a state transition to <see cref="TimerState.Stopped"/>).
        /// </summary>
        public const int STANDARD_EVENT_STOPPED = 4;
    
        /// <summary>
        /// Signifies that the timer has been reset.
        ///
        /// Resets do not transition the state of a timer on their own. They merely indicate that the timer has been reset
        /// back to the beginning.
        /// </summary>
        public const int STANDARD_EVENT_RESET = 5;
        
        public const int STANDARD_EVENT_COUNT = 6;
    }
}