using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Micron
{
    public abstract class JobData
    {
        /// <summary>
        /// The name of this job
        /// </summary>
        public string Name{get; init;} = "";

        /// <summary>
        /// The cron expression that schedules the job.
        /// If not set then you must specify "Days" and "At"
        /// </summary>
        public string Cron{get; init;} = "";

        /// <summary>
        /// The days to run on, if the cron expression is not set
        /// </summary>
        public DaysOfTheWeek Days{get; init;} = DaysOfTheWeek.None;

        /// <summary>
        /// The time of day to run on, if the cron expression is not set
        /// </summary>
        public TimeSpan At{get; init;} = TimeSpan.MinValue;
    }
}
