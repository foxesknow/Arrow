using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading
{
    /// <summary>
    /// A simple work dispatcher that allows any thread that calls DispatcherLoop to handle queued work
    /// </summary>
    public class SimpleWorkDispatcher : IWorkDispatcher, IDisposable
    {
        private readonly object m_Lock = new object();
        private Queue<Action> m_Work = new Queue<Action>();

        private bool m_Running = true;

        /// <summary>
		/// Adds a request to the work dispatcher
		/// </summary>
		/// <param name="waitCallback">The delegate that will be called</param>
		/// <returns>true if the request was queued, false otherwise</returns>
        public bool QueueUserWorkItem(WaitCallback waitCallback)
        {
            return QueueUserWorkItem(waitCallback, null);   
        }

        /// <summary>
		/// Adds a request to the work queue
		/// </summary>
		/// <param name="waitCallback">The delegate that will be called</param>
		/// <param name="state">Any additional state information for the request</param>
		/// <returns>true if the request was queued, false otherwise</returns>
        public bool QueueUserWorkItem(WaitCallback waitCallback, object? state)
        {
            if(waitCallback == null) throw new ArgumentNullException(nameof(waitCallback));

            lock(m_Lock)
            {
                if(m_Running == false) return false;

                m_Work.Enqueue(() => waitCallback(state));
                Monitor.Pulse(m_Lock);

                return true;
            }
        }        

        /// <summary>
        /// Loops around waiting for work to become available and executes any that does
        /// </summary>
        public void DispatcherLoop()
        {
            while(true)
            {
                Action? action = null;
                
                lock(m_Lock)
                {
                    while(m_Work.Count == 0 && m_Running)
                    {
                        Monitor.Wait(m_Lock);
                    }

                    if(m_Running == false) return;

                    action = m_Work.Dequeue();
                }

                action();
            }
        }

        /// <summary>
        /// Stops the work dispatcher.
        /// Any threads waiting in DispatherLoop will return
        /// </summary>
        public void Stop()
        {
            lock(m_Lock)
            {
                m_Running = false;
                Monitor.PulseAll(m_Lock);
            }
        }

        /// <summary>
        /// Stops the work dispatcher
        /// </summary>
        public void Dispose()
        {
            Stop();
        }
    }
}
