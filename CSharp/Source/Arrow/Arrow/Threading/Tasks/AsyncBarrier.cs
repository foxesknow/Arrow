using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// A barrier that will allow a set number of threads to go when signaled.
    /// More threads that participants can singal the barrier, but only the 
    /// specified number of participants will be allows to advance at any one time
    /// </summary>
    public class AsyncBarrier
    {
        private readonly int m_NumberOfParticipants;
        private int m_OutstandingParticipants;

        private AsyncEventWaitHandle m_Handle = new AsyncManualResetEvent(false);
        private readonly object m_SyncRoot = new object();

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="numberOfParticipants">The number of threads that will wait at the barrier before moving forward</param>
        public AsyncBarrier(int numberOfParticipants)
        {
            if(numberOfParticipants <= 0) throw new ArgumentException("need at least one participant", nameof(numberOfParticipants));

            m_NumberOfParticipants = numberOfParticipants;
            m_OutstandingParticipants = numberOfParticipants;
        }

        /// <summary>
        /// Signals that the caller is ready to wait at the barrier and returns a task that will be complete when everyone is at the barrier
        /// </summary>
        /// <returns></returns>
        public Task SignalAndWait()
        {
            AsyncEventWaitHandle handle = null;

            lock(m_SyncRoot)
            {
                m_OutstandingParticipants--;

                if(m_OutstandingParticipants == 0)
                {
                    // We're done. We'll set the handle outside the lock, but we need to reset our state
                    handle = m_Handle;
                    m_Handle = new AsyncManualResetEvent(false);
                    m_OutstandingParticipants = m_NumberOfParticipants;
                }
                else
                {
                    // We're still waiting for more to signal
                    return m_Handle.WaitAsync();
                }
            }

            handle.Set();
            return handle.WaitAsync();
        }
    }
}
