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
        private TaskCompletionSource<bool> m_Source = new TaskCompletionSource<bool>();

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
            // Use TrySetResult so that multiple callers can call it without throwing an exception
            var source = InterlockedEx.Read(ref m_Source);
            source.TrySetResult(true);
        }

        /// <summary>
        /// Waits for the event to be signaled
        /// </summary>
        /// <returns></returns>
        public override Task WaitAsync()
        {
            return InterlockedEx.Read(ref m_Source).Task;
        }

        /// <summary>
        /// Resets the event back to unsignaled
        /// </summary>
        public override void Reset()
        {
            var source = m_Source;

            while(true)
            {
                if(source.Task.IsCompleted == false)
                {
                    // It's not completed (eg set) so we don't need to replace it
                    return;
                }

                var previous = Interlocked.CompareExchange(ref m_Source, new TaskCompletionSource<bool>(), source);
                if(previous == source)
                {
                    return;
                }

                source = previous;
            }
        }
    }
}
