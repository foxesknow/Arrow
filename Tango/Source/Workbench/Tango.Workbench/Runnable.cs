using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Calendar;
using Arrow.Logging;
using Arrow.Logging.Loggers;

namespace Tango.Workbench
{
    public abstract class Runnable
    {
        private readonly static ILog s_RootLog = LogManager.GetDefaultLog();

        /// <summary>
        /// Returns a completed task that you can await on to allow you to prefix methods with async
        /// </summary>
        /// <returns></returns>
        protected ValueTask ForceAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// The log that the job should use to log what it is doing
        /// </summary>
        protected internal ILog Log{get; private set;} = s_RootLog;

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
        /// True if the job should be verbose with its logging.
        /// This can be overridden from the command line or by the containing group.
        /// </summary>
        public bool Verbose{get; set;}

        /// <summary>
        /// The score for the current job
        /// </summary>
        protected internal Score Score{get; private set;} = default!;
       
        /// <summary>
        /// Context information for the job.
        /// This is only valid when your job is running
        /// </summary>
        protected internal JobContext Context{get; private set;} = default!;

        internal virtual void RegisterRuntimeDependencies(RuntimeDependencies dependencies)
        {
            this.Context = dependencies.Context;
            this.Score = dependencies.Score;
        }

        internal virtual void UnregisterRuntimeDependencies()
        {
            this.Context = null!;
            this.Score = null!;
        }

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

        /// <summary>
        /// Creates a new expander with the date set to the local date
        /// </summary>
        /// <returns></returns>
        protected Expander MakeExpander()
        {
            return new Expander().AddDates(Clock.Now);
        }
    }
}
