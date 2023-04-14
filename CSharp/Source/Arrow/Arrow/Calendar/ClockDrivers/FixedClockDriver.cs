using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Calendar.ClockDrivers
{
    /// <summary>
    /// A clock that always returns the same time
    /// </summary>
    public class FixedClockDriver : IClockDriver
    {
        private readonly DateTime m_Local;
        private readonly DateTime m_Utc;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="dateTime">The date and time to always return</param>
        public FixedClockDriver(DateTime dateTime)
        {
            m_Local = dateTime.ToLocalTime();
            m_Utc = dateTime.ToUniversalTime();
        }

        /// <summary>
        /// Returns the local time
        /// </summary>
        public DateTime Now
        {
            get { return m_Local; }
        }

        /// <summary>
        /// Returns the utc time
        /// </summary>
        public DateTime UtcNow
        {
            get { return m_Utc; }
        }

        /// <summary>
        /// Renders the clock as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Local={0}, Utc={1}", m_Local, m_Utc);
        }
    }
}
