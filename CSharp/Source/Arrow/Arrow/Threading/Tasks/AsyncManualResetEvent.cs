using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// An asynchronous manual reset event.
    /// Multiple threads can wait on the event, and all will be released when the event is signaled
    /// </summary>
    public class AsyncManualResetEvent : AsyncEventWaitHandle
    {
        private volatile TaskCompletionSource<bool> m_Source = MakeTcs();

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="initiallySignaled">true if the event is initially signaled, false to set the event to nonsignaled</param>
        public AsyncManualResetEvent(bool initiallySignaled)
        {
            if(initiallySignaled)
            {
                m_Source.SetResult(true);
            }
        }

        /// <summary>
        /// Sets the event, waking any threads waiting on the event
        /// </summary>
        public override void Set()
        {
            var source = m_Source;
            ReleaseTcs(source);
        }

        /// <summary>
        /// Waits for the event to be signaled
        /// </summary>
        /// <returns></returns>
        public override Task WaitAsync()
        {
            return m_Source.Task;
        }

        /// <summary>
        /// Resets the event back to unsignaled
        /// </summary>
        public override void Reset()
        {
            while(true)
            {
                var tcs = m_Source;
                if(tcs.Task.IsCompleted == false) return;

                if(Interlocked.CompareExchange(ref m_Source, MakeTcs(), tcs) == tcs)
                {
                    return;
                }
            }
        }
    }
}
