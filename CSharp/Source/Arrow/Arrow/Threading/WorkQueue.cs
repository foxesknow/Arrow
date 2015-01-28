using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading
{
	/// <summary>
	/// Defines a work queue that allows for multiple producers and a single consumer
	/// </summary>
	/// <typeparam name="T">The type of work to be queued</typeparam>
	public abstract class WorkQueue<T> : IWorkQueue<T>
	{
		private readonly object m_SyncRoot=new object();
		
		private readonly EventWaitHandle m_StopEvent=new AutoResetEvent(false);
		
		/// <summary>
		/// A list is used instead of a queue as it performs better is our swap/process usage model
		/// </summary>
		private List<T> m_PendingWork;
		
		// Although it's slightly faster to allocate a new list in the ProcessQueue
		// method it's better from a GC perspective to keep the list around
		private List<T> m_SwapData;
		
		private readonly int m_InitialCapacity;
		
		private readonly IWorkDispatcher m_Dispatcher;
		
		private bool m_ThreadActive;
		
		private bool m_StopProcessing;
		
		private bool m_Disposed;
		
		/// <summary>
		/// Initializes the instance using the default work item dispatcher
		/// </summary>
		protected WorkQueue() : this(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		protected WorkQueue(IWorkDispatcher dispatcher) : this(dispatcher,8)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		/// <param name="initialCapacity">The initial capacity for the queue</param>
		protected WorkQueue(IWorkDispatcher dispatcher, int initialCapacity)
		{
			if(initialCapacity<0) throw new ArgumentOutOfRangeException("initialCapacity");
		
			if(dispatcher==null) dispatcher=new ThreadPoolWorkDispatcher();
			
			m_Dispatcher=dispatcher;
			m_InitialCapacity=initialCapacity;
			
			m_PendingWork=new List<T>(initialCapacity);
			m_SwapData=new List<T>(initialCapacity);
		}
		
		/// <summary>
		/// Adds a new item to the queue
		/// </summary>
		/// <param name="item">The item to process</param>
		public void Enqueue(T item)
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
		public bool TryEnqueue(T item)
		{
			lock(m_SyncRoot)
			{
				if(m_Disposed) return false;
				
				DoEnqueue(item);
				return true;
			}
		}
		
		/// <summary>
		/// Enqueues an item if there is currently a thread running
		/// to process items, otherwise the item is processed on the
		/// current thread
		/// </summary>
		/// <param name="item">The data item to process</param>
		public void EnqueueOrExecute(T item)
		{
			lock(m_SyncRoot)
			{
				if(m_Disposed) throw new ObjectDisposedException("WorkDispatchQueue");
			
				if(m_ThreadActive)
				{
					DoEnqueue(item);
				}
				else
				{
					Process(item);
				}
			}
		}

		private void DoEnqueue(T item)
		{
			if(m_Disposed) throw new ObjectDisposedException("WorkDispatchQueue");
				
			m_PendingWork.Add(item);
				
			if(m_ThreadActive==false)
			{
				m_ThreadActive=true;
				m_Dispatcher.QueueUserWorkItem(ProcessQueue);
			}
		}
		
		/// <summary>
		/// Returns the work item dispatcher being used to schedule threads
		/// </summary>
		public IWorkDispatcher WorkItemDispatcher
		{
			get{return m_Dispatcher;}
		}
		
		/// <summary>
		/// Returns the number of items in the queue
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_PendingWork.Count;
				}
			}
		}
		
		/// <summary>
		/// Stops processing and releases any resources
		/// </summary>
		public void Dispose()
		{
			bool shouldWait=false;
			
			lock(m_SyncRoot)
			{
				if(m_Disposed) return;
			
				m_StopProcessing=true;
				m_Disposed=true;
				
				// We only need to block if the thread is currently running
				if(m_ThreadActive) shouldWait=true;
			}		
			
			if(shouldWait) m_StopEvent.WaitOne();
			
			m_StopEvent.Close();
		}
		
		/// <summary>
		/// Removes any pending work items from the work queue
		/// </summary>
		public void Clear()
		{
			lock(m_SyncRoot)
			{
				m_PendingWork.Clear();
			}
		}
		
		/// <summary>
		/// Waits for any outstanding work to be processed and shuts the queue down
		/// </summary>
		public void Close()
		{
			Dispose();
		}
		
		/// <summary>
		/// Switches the pending work queue with an empty queue
		/// </summary>
		/// <returns>The work that should be run next</returns>
		private List<T> SwitchData()
		{
			var workToProcess=m_PendingWork;
			m_PendingWork=m_SwapData;
			m_SwapData=workToProcess;

			return workToProcess;
		}

		/// <summary>
		/// Processes all the items in the queue
		/// </summary>
		/// <param name="state">Not used</param>
		private void ProcessQueue(object state)
		{
			lock(m_SyncRoot)
			{
				try
				{
					while(m_PendingWork.Count!=0 && m_StopProcessing==false)
					{
						var workToProcess=SwitchData();
					
						// We can release the lock here
						Monitor.Exit(m_SyncRoot);
						
						try
						{
							ProcessItems(workToProcess);
						}
						finally
						{					
							// We need to remove the data from the swap
							workToProcess.Clear();							
							
							// And re-acquire it here before we loop back around
							// We do it in a finally block to remain consistent
							// if a ThreadAbortException is thrown
							Monitor.Enter(m_SyncRoot);
						}
					}
				}
				finally
				{				
					// When we're about to leave we reset the active state so that a new thread can be launched
					// This is done in a finally block in case we get a ThreadAbortException exception
					m_ThreadActive=false;
					
					if(m_StopProcessing) 
					{
						// Process anything that's left
						ProcessItems(m_PendingWork);
						
						m_StopEvent.Set();
					}
				}
			}
		}
		
		private void ProcessItems(List<T> items)
		{
			for(int i=0; i<items.Count; i++)
			{
				var item=items[i];
				Process(item);
			}
		}
		
		/// <summary>
		/// Runs an individual worker.
		/// This method is guaranteed not to throw an exception
		/// </summary>
		/// <param name="data">The data to process</param>
		protected abstract void Process(T data);	
	}
}
