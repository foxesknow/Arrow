using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arrow.Calendar.ClockDrivers;

namespace Arrow.Calendar
{
    /// <summary>
    /// Provides access to the date and time
    /// 
    /// By using this class instead of calling DateTime.Now directly you can
    /// abstract away the notion of time. This is useful in areas such as
    /// unit testing.
    /// 
    /// All Arrow components should use the class when the current date or time is required
    /// </summary>
    public static class Clock
    {
        private static IClockDriver s_Clock = new SystemClockDriver();

        /// <summary>
        /// Returns the current date time
        /// </summary>
        public static DateTime Now
        {
            get{return s_Clock.Now;}
        }

        /// <summary>
        /// Returns a utc date time for now
        /// </summary>
        public static DateTime UtcNow
        {
            get{return s_Clock.UtcNow;}
        }

        /// <summary>
        /// The current clock driver being used
        /// </summary>
        public static IClockDriver ClockDriver
        {
            get{return s_Clock;}
            set
            {
                if(value is null) throw new ArgumentNullException("value");
                s_Clock = value;
            }
        }
    }
}
