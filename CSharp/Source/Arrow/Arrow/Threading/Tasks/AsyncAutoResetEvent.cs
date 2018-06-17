using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// An asynchronous auto reset event.
    /// Multiple threads can wait on the event, but only one will be released when the event is signaled
    /// </summary>
    public class AsyncAutoResetEvent : AsyncEventWaitHandle
    {
        private bool m_Signaled;
        private readonly Queue<TaskCompletionSource<bool>> m_PendingWaits = new Queue<TaskCompletionSource<bool>>();

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="initiallySignaled">true if the event is initially signaled, false to set the event to nonsignaled</param>
        public AsyncAutoResetEvent(bool initiallySignaled)
        {
            m_Signaled = initiallySignaled;
        }

        /// <summary>
        /// Sets the event back to unsignaled
        /// </summary>
        public override void Reset()
        {
            lock(m_PendingWaits)
            {
                m_Signaled = false;
            }
        }

        /// <summary>
        /// Signals the event.
        /// If anyone is waiting then they will be released and the event will be reset,
        /// otherwise the event will stay signaled until someone waits on it
        /// </summary>
        public override void Set()
        {
            TaskCompletionSource<bool> callerToRelease = null;

            lock(m_PendingWaits)
            {
                if(m_PendingWaits.Count != 0)
                {
                    // There's someone waiting, so just release them
                    callerToRelease = m_PendingWaits.Dequeue();
                }
                else
                {
                    // Nobody is waiting, so remember we're signaled
                    m_Signaled = true;
                }
            }

            // If there was someone waiting then release them
            if(callerToRelease != null)
            {
                SetTaskCompletionSouce(callerToRelease);
            }
        }

        public override Task WaitAsync()
        {
            lock(m_PendingWaits)
            {
                if(m_Signaled)
                {
                    // Easy, we've been signaled so just return success
                    m_Signaled = false;
                    return Completed;
                }
                else
                {
                    // We've not been signaled yet, so place the caller on a queue
                    // waiting to be signaled
                    var source = new TaskCompletionSource<bool>();
                    m_PendingWaits.Enqueue(source);

                    return source.Task;
                }
            }
        }
    }
}
