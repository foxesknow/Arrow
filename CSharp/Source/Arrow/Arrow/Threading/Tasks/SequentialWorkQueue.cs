using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Executes asynchronous functions on a work queue, waiting for the function to complete
    /// before running the next function.
    /// </summary>
    public sealed class SequentialWorkQueue : IWorkQueue<Func<ValueTask>>
    {
        private readonly object m_SyncRoot = new object();

        private readonly EventWaitHandle m_StopEvent = new AutoResetEvent(false);

        /// <summary>
        /// A list is used instead of a queue as it performs better is our swap/process usage model
        /// </summary>
        private List<Func<ValueTask>> m_PendingWork;

        // Although it's slightly faster to allocate a new list in the ProcessQueue
        // method it's better from a GC perspective to keep the list around
        private List<Func<ValueTask>> m_SwapData;

        private readonly int m_InitialCapacity;

        private readonly IWorkDispatcher m_Dispatcher;

        private bool m_ThreadActive;

        private bool m_StopProcessing;

        private bool m_Disposed;

        /// <summary>
        /// Initializes the instance using the default work item dispatcher
        /// </summary>
        public SequentialWorkQueue() : this(null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
        public SequentialWorkQueue(IWorkDispatcher? dispatcher) : this(dispatcher, 8)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
        /// <param name="initialCapacity">The initial capacity for the queue</param>
        public SequentialWorkQueue(IWorkDispatcher? dispatcher, int initialCapacity)
        {
            if(initialCapacity < 0) throw new ArgumentOutOfRangeException("initialCapacity");
            if(dispatcher is null) dispatcher = new ThreadPoolWorkDispatcher();

            m_Dispatcher = dispatcher;
            m_InitialCapacity = initialCapacity;

            m_PendingWork = new(initialCapacity);
            m_SwapData = new(initialCapacity);
        }

        public void Dispose()
        {
            var shouldWait = false;

            lock(m_SyncRoot)
            {
                if(m_Disposed) return;

                m_StopProcessing = true;
                m_Disposed = true;

                // We only need to block if the thread is currently running
                if(m_ThreadActive) shouldWait = true;
            }

            if(shouldWait) m_StopEvent.WaitOne();

            m_StopEvent.Close();
        }

        /// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public void Enqueue(Func<ValueTask> item)
        {
            lock(m_SyncRoot)
            {
                DoEnqueue(item);
            }
        }

        /// <summary>
        /// Attempts to add a new item to the queue.
        /// This method will fail to enqueue an item if the queue has been disposed
        /// </summary>
        /// <param name="item">The item to process</param>
        /// <returns>true if the item was enqueued, false if it wasn't</returns>
        public bool TryEnqueue(Func<ValueTask> item)
        {
            lock(m_SyncRoot)
            {
                if(m_Disposed) return false;

                DoEnqueue(item);
                return true;
            }
        }

        private void DoEnqueue(Func<ValueTask> item)
        {
            if(m_Disposed) throw new ObjectDisposedException("WorkDispatchQueue");

            m_PendingWork.Add(item);

            if(m_ThreadActive == false)
            {
                m_ThreadActive = true;
                m_Dispatcher.QueueUserWorkItem(ProcessQueue);
            }
        }

        /// <summary>
        /// Switches the pending work queue with an empty queue
        /// </summary>
        /// <returns>The work that should be run next</returns>
        private List<Func<ValueTask>> SwitchData()
        {
            var workToProcess = m_PendingWork;
            m_PendingWork = m_SwapData;
            m_SwapData = workToProcess;

            return workToProcess;
        }

        /// <summary>
        /// Processes all the items in the queue
        /// </summary>
        /// <param name="state">Not used</param>
        private async void ProcessQueue(object? state)
        {
            // We can't use a lock as we await within it, so we've got to do it manually
            var lockTaken = false;

            try
            {
                Monitor.Enter(m_SyncRoot, ref lockTaken);

                while(lockTaken && m_PendingWork.Count != 0 && m_StopProcessing == false)
                {
                    var workToProcess = SwitchData();

                    // We can release the lock here
                    Monitor.Exit(m_SyncRoot);

                    try
                    {
                        await ProcessItems(workToProcess).ContinueOnAnyContext();
                    }
                    finally
                    {
                        // We need to remove the data from the swap
                        workToProcess.Clear();

                        // And re-acquire it here before we loop back around
                        lockTaken = false;
                        Monitor.Enter(m_SyncRoot, ref lockTaken);
                    }
                }
            }
            finally
            {
                if(lockTaken)
                {
                    // We're still holding the lock, and we must only switch lists when locked
                    m_ThreadActive = false;
                    var stopProcessing = m_StopProcessing;

                    // If we're stopping then we need to run anything left
                    var workToProcess = (stopProcessing ? SwitchData() : null);

                    // Whatever we're doing we can give up the lock now
                    Monitor.Exit(m_SyncRoot);

                    if(stopProcessing)
                    {
                        if(workToProcess is not null && workToProcess.Count != 0)
                        {
                            await ProcessItems(workToProcess).ContinueOnAnyContext();
                        }

                        m_StopEvent.Set();
                    }
                }
            }
        }

        private async ValueTask ProcessItems(List<Func<ValueTask>> items)
        {
            for(var i = 0; i < items.Count; i++)
            {
                var function = items[i];

                try
                {
                    await function().ContinueOnAnyContext();
                }
                catch
                {
                    // We can't do anything with it
                }

            }
        }
    }
}
