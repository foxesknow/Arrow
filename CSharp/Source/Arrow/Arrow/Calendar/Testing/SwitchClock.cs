using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Arrow.Calendar.Testing
{
    /// <summary>
    /// Switches to a new clock.
    /// This is useful for unit testing
    /// </summary>
    public static class SwitchClock
    {
        /// <summary>
        /// Switches to a new clock
        /// </summary>
        /// <param name="clock"></param>
        /// <returns></returns>
        public static IDisposable To(IClock clock)
        {
            var currentClock = Clock.ClockDriver;
            Clock.ClockDriver = clock;

            return Disposer.Make(() => Clock.ClockDriver = currentClock);
        }
    }
}
