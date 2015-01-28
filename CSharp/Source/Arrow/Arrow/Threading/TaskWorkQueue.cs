using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading
{
	/// <summary>
	/// A work queue that returns Task instances
	/// </summary>
	public partial class TaskWorkQueue : IDisposable
	{
		private readonly object m_SyncRoot=new object();
		
		private readonly EventWaitHandle m_StopEvent=new AutoResetEvent(false);
		
		/// <summary>
		/// A list is used instead of a queue as it performs better is our swap/process usage model
		/// </summary>
		private List<IWork> m_PendingWork;
		
		// Although it's slightly faster to allocate a new list in the ProcessQueue
		// method it's better from a GC perspective to keep the list around
		private List<IWork> m_SwapData;
		
		private readonly int m_InitialCapacity;
		
		private readonly IWorkDispatcher m_Dispatcher;
		
		private bool m_ThreadActive;
		
		private bool m_StopProcessing;
		
		private bool m_Disposed;

		/// <summary>
		/// Initializes the instance using the default work item dispatcher
		/// </summary>
		public TaskWorkQueue() : this(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		public TaskWorkQueue(IWorkDispatcher dispatcher) : this(dispatcher,8)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		/// <param name="initialCapacity">The initial capacity for the queue</param>
		public TaskWorkQueue(IWorkDispatcher dispatcher, int initialCapacity)
		{
			if(initialCapacity<0) throw new ArgumentOutOfRangeException("initialCapacity");
		
			if(dispatcher==null) dispatcher=new ThreadPoolWorkDispatcher();
			
			m_Dispatcher=dispatcher;
			m_InitialCapacity=initialCapacity;
			
			m_PendingWork=new List<IWork>(initialCapacity);
			m_SwapData=new List<IWork>(initialCapacity);
		}

		/// <summary>
		/// Schedules a function for running on the work queue
		/// </summary>
		/// <typeparam name="T">The type of data returned by the function</typeparam>
		/// <param name="function">The function to execute</param>
		/// <returns>A task that allows you to synchronize on the execution of the function</returns>
		public Task<T> Enqueue<T>(Func<T> function)
		{
			if(function==null) throw new ArgumentException("function");

			var work=new Work<T>(function);
			DoEnqueue(work);

			return work.Task;
		}

		/// <summary>
		/// Schedules an action for running on the work queue
		/// </summary>
		/// <param name="action">The action to execute</param>
		/// <returns>A task that allows you to synchronize on the execution of the function</returns>
		public Task Enqueue(Action action)
		{
			if(action==null) throw new ArgumentException("action");

			var work=new Work(action);
			DoEnqueue(work);

			return work.Task;
		}

		private void DoEnqueue(IWork work)
		{
			if(m_Disposed) throw new ObjectDisposedException("WorkDispatchQueue");
				
			m_PendingWork.Add(work);
				
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
		/// Cancels any work tasks that are enqueued but not yet scheduled to run.
		/// This will move the Task into a cancelled state.
		/// 
		/// Calls to wait or retrive state from the task will throw an AggregateException
		/// with a TaskCanceledException in the inner exception.
		/// </summary>
		public void Cancel()
		{
			List<IWork> workItems=null;

			// Do the cancel outside the lock to allow re-entrancy on other threads
			lock(m_SyncRoot)
			{
				workItems=m_PendingWork;
				m_PendingWork=new List<IWork>();
			}

			foreach(var work in workItems)
			{
				work.Cancel();
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
		private List<IWork> SwitchData()
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
						m_PendingWork.Clear();
						
						m_StopEvent.Set();
					}
				}
			}
		}
		
		private void ProcessItems(List<IWork> items)
		{
			for(int i=0; i<items.Count; i++)
			{
				var work=items[i];
				work.Execute();
			}
		}		
	}
}
