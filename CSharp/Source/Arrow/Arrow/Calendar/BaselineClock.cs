using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Calendar
{
    /// <summary>
    /// A clock that always returns a time relative to a baseline time.
    /// </summary>
    public class BaselineClock : IClock
    {
        private readonly DateTime m_UtcBaseline;
        private readonly TimeSpan m_Delta;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="baseline">The date and time that will be the basis of time</param>
        public BaselineClock(DateTime baseline)
        {
            m_UtcBaseline = baseline.ToUniversalTime();
            m_Delta = DateTime.UtcNow - m_UtcBaseline;
        }

        /// <summary>
        /// Returns the local time
        /// </summary>
        public DateTime Now
        {
            get
            {
                return this.UtcNow.ToLocalTime();
            }
        }

        /// <summary>
        /// Returns the utc time
        /// </summary>
        public DateTime UtcNow
        {
            get
            {
                DateTime date = DateTime.UtcNow - m_Delta;
                return date;
            }
        }

        /// <summary>
        /// Renders the clock as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("Baseline={0}, Delta={1}", m_UtcBaseline, m_Delta);
        }
    }
}
