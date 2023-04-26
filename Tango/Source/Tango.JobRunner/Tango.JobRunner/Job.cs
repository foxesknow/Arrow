using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Logging;
using Arrow.Logging.Loggers;

namespace Tango.JobRunner
{
    public abstract class Job
    {
        private readonly static ILog s_RootLog = LogManager.GetDefaultLog();
        
        /// <summary>
        /// Runs the actual job
        /// </summary>
        /// <returns></returns>
        public abstract ValueTask Run();

        /// <summary>
        /// The log that the job should use to log what it is doing
        /// </summary>
        public ILog Log{get; private set;} = s_RootLog;

        /// <summary>
        /// A log for verbose output, if verbose is enabled
        /// </summary>
        protected ILog VerboseLog
        {
            get
            {
                return this.Verbose ? this.Log : NullLog.Instance;
            }
        }

        /// <summary>
        ///  A user defined name for the job
        /// </summary>
        public string? Name{get; set;}

        /// <summary>
        /// True if the job should be verbose with its logging
        /// </summary>
        public bool Verbose{get; set;}

        /// <summary>
        /// The score for the current job
        /// </summary>
        protected internal Score Score{get; internal set;} = default!;
       

        protected internal JobContext Context{get; internal set;} = default!;

        internal void SetLogName(string? name)
        {
            Log = MakeLog(name);
        }

        public override string ToString()
        {
            return Name ?? "No name";
        }

        /// <summary>
        /// Creates a log based on the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private ILog MakeLog(string? name)
        {
            if(string.IsNullOrEmpty(name))
            {
                return s_RootLog;
            }

            var prefix = $"[{name}]";
            return new PrefixLog(s_RootLog, prefix);
        }
    }
}
