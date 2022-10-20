using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Raised whan the async work queue encounters an error it can't report on a task
    /// </summary>
    public sealed class UnhandledAsyncWorkQueueExceptionEventArgs : EventArgs
    {
        public UnhandledAsyncWorkQueueExceptionEventArgs(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception{get;}

        /// <summary>
        /// True to have the queue rethrow the exception, false to swallow it.
        /// The default is false
        /// </summary>
        public bool Rethrow{get;}
    }
}
