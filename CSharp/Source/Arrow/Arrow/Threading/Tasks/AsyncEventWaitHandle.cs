using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Base class for async event wait handles
    /// </summary>
    public abstract class AsyncEventWaitHandle : AsyncWaitHandle
    {
        /// <summary>
        /// Sets the event
        /// </summary>
        public abstract void Set();

        /// <summary>
        /// Resets the event
        /// </summary>
        public abstract void Reset();
    }
}
