using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Arrow.Threading
{
    /// <summary>
    /// A custom SynchronizationContext that posts work to a IWorkDispatcher for processing
    /// </summary>
    public class WorkDispatcherSynchronizationContext : SynchronizationContext
    {
        private readonly IWorkDispatcher m_WorkDispatcher;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="workDispatcher">Thr work dispatcher that will schedule the work for execution</param>
        public WorkDispatcherSynchronizationContext(IWorkDispatcher workDispatcher)
        {
            if(workDispatcher==null) throw new ArgumentNullException(nameof(workDispatcher));

            m_WorkDispatcher=workDispatcher;
        }

        /// <summary>
        /// Posts work for processing asychronously
        /// </summary>
        /// <param name="d">The method to execute</param>
        /// <param name="state">State information for the method</param>
        public override void Post(SendOrPostCallback d, object state)
        {
            m_WorkDispatcher.QueueUserWorkItem(s=>
            {
                d(s);
            },state);
        }

        /// <summary>
        /// Posts work for processing sychronously
        /// </summary>
        /// <param name="d">The method to execute</param>
        /// <param name="state">State information for the method</param>
        public override void Send(SendOrPostCallback d, object state)
        {
            d(state);
        }

        /// <summary>
        /// Creates a copy of the context
        /// </summary>
        /// <returns>A new copy of the context</returns>
        public override SynchronizationContext CreateCopy()
        {
            return new WorkDispatcherSynchronizationContext(m_WorkDispatcher);
        }

        /// <summary>
        /// Install a new synchronization context and returns a disposer to easily reinstall the previous one
        /// </summary>
        /// <param name="synchronizationContext">The context to install</param>
        /// <returns>A disposable implementation that will reinstall the existing context</returns>
        public static IDisposable Install(SynchronizationContext synchronizationContext)
        {
            if(synchronizationContext==null) throw new ArgumentNullException(nameof(synchronizationContext));

            var current=SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(synchronizationContext);

            return new Disposer(()=>
            {
                SynchronizationContext.SetSynchronizationContext(current);
            });
        }
    }
}
