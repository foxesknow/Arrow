using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Base class for anything that functions like an awaitable handle
    /// </summary>
    public abstract class AsyncWaitHandle
    {
        /// <summary>
        /// A task that has always completed successfully
        /// </summary>
        protected static readonly Task Completed = Task.FromResult(true);

        /// <summary>
        /// Returns a task that will be completed at some point
        /// </summary>
        /// <returns></returns>
        public abstract Task WaitAsync();
    }
}
