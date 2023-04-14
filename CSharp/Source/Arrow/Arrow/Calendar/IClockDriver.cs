using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Calendar
{
    /// <summary>
    /// Defines a clock that returns the current time
    /// </summary>
    public interface IClockDriver
    {
        /// <summary>
        /// Returns the local time
        /// </summary>
        DateTime Now{get;}

        /// <summary>
        /// Returns the utc time
        /// </summary>
        DateTime UtcNow{get;}
    }
}
