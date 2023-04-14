using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Calendar.ClockDrivers
{
    /// <summary>
    /// Returns the current date/time from the DateTime class
    /// </summary>
    public sealed class SystemClockDriver : IClockDriver
    {
        /// <summary>
        /// Returns DateTime.Now
        /// </summary>
        public DateTime Now
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// Returns DateTime.UtcNow
        /// </summary>
        public DateTime UtcNow
        {
            get { return DateTime.UtcNow; }
        }

        public override string ToString()
        {
            return "SystemClock";
        }
    }
}
