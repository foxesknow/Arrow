using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Calendar.ClockDrivers
{
    /// <summary>
    /// Manages the global clock exposed by the Clock class
    /// </summary>
    public static class GlobalClockDriverManager
    {
        /// <summary>
        /// Installs a new clock driver for the global Clock instance
        /// </summary>
        /// <param name="clockDriver"></param>
        /// <returns>The previously installed clock driver</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IClockDriver Install(IClockDriver clockDriver)
        {
            if(clockDriver is null) throw new ArgumentNullException(nameof(clockDriver));

            return Clock.Install(clockDriver);
        }

        /// <summary>
        /// Returns the currently installed global clock driver used by the Clock instance
        /// </summary>
        /// <returns></returns>
        public static IClockDriver Current()
        {
            return Clock.Driver;
        }
    }
}
