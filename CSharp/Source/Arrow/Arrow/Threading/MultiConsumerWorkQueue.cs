using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading
{
	/// <summary>
	/// A queue that has multiple consumers to process work items.
	/// 
	/// The typical use of this queue is for short bursts of processing 
	/// rather than as a form of offloading work to another stage in the system.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class MultiConsumerWorkQueue<T> : IWorkQueue<T>
	{
		private readonly object m_SyncRoot=new object();
		
		private readonly IWorkDispatcher m_Dispatcher;		
		private readonly int m_NumberOfConsumers;

		private readonly Queue<T> m_Queue=new Queue<T>();

		private bool m_StopProcessing;
		private bool m_Disposed;

		private readonly object m_ActiveConsumerSyncRoot=new object();
		private int m_ActiveConsumers;

		/// <summary>
		/// Initializes the instance using the default work item dispatcher
		/// </summary>
		protected MultiConsumerWorkQueue() : this(null,2)
		{
		}

		/// <summary>
		/// Initializes the instance using the default work item dispatcher
		/// <param name="numberOfConsumers">The number of consumer threads for the work</param>
		/// </summary>
		protected MultiConsumerWorkQueue(int numberOfConsumers) : this(null,numberOfConsumers)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		/// <param name="numberOfConsumers">The number of consumer threads for the work</param>
		protected MultiConsumerWorkQueue(IWorkDispatcher? dispatcher, int numberOfConsumers)
		{
			if(numberOfConsumers<=0) throw new ArgumentOutOfRangeException("numberOfConsumers");
			if(dispatcher==null) dispatcher=new ThreadPoolWorkDispatcher();
			
			m_Dispatcher=dispatcher;
			m_NumberOfConsumers=numberOfConsumers;	
			
			StartConsumers();		
		}

		/// <summary>
		/// Adds work to the queue for processing
		/// </summary>
		/// <param name="work">The work to process</param>
		public void Enqueue(T work)
		{
			lock(m_SyncRoot)
			{
				if(m_Disposed) throw new ObjectDisposedException("MultiConsumerWorkQueue");

				m_Queue.Enqueue(work);
				Monitor.Pulse(m_SyncRoot);
			}
		}

		/// <summary>
		/// Removes and existing work from the queue
		/// </summary>
		public void Clear()
		{
			lock(m_SyncRoot)
			{
				if(m_Disposed) throw new ObjectDisposedException("MultiConsumerWorkQueue");
				m_Queue.Clear();
			}
		}

		/// <summary>
		/// Returns the number of items queued for processing
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_Queue.Count;
				}
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
		/// Disposes of the queue. Any pending work is processed first
		/// </summary>
		public void Dispose()
		{
			lock(m_SyncRoot)
			{
				if(m_Disposed) return;
				
				m_StopProcessing=true;
				m_Disposed=true;

				// Wake up the worker threads so they can start to shut down.
				Monitor.PulseAll(m_SyncRoot);
			}

			lock(m_ActiveConsumerSyncRoot)
			{
				// Wait for any works to shut down
				while(m_ActiveConsumers!=0)
				{
					Monitor.Wait(m_ActiveConsumerSyncRoot);
				}
			}
		}

		private void StartConsumers()
		{
			m_ActiveConsumers=m_NumberOfConsumers;

			for(int i=0; i<m_NumberOfConsumers; i++)
			{
				m_Dispatcher.QueueUserWorkItem(ProcessQueue);
			}
		}

		/// <summary>
		/// Attempts to remove an item from the work queue.
		/// This method determines when the caller should stop dequeuing and exit
		/// </summary>
		/// <param name="data">On success the data to process</param>
		/// <returns>The outcome of the call</returns>
		private DequeueResult Dequeue(out T data)
		{
			lock(m_SyncRoot)
			{
				// We only stop when asked and there's nothing left to do.
				// To cancel and exit a user should call Clear()
				if(m_StopProcessing && m_Queue.Count==0)
				{
					data=default!;
					return DequeueResult.StopProcessing;
				}

				while(m_Queue.Count==0)
				{
					Monitor.Wait(m_SyncRoot);

					// We've been woken up, but it may be because we need to stop
					if(m_StopProcessing && m_Queue.Count==0)
					{
						data=default!;
						return DequeueResult.StopProcessing;
					}
				}

				data=m_Queue.Dequeue();
				return DequeueResult.Process;
			}
		}

		/// <summary>
		/// Processes items in the queue
		/// </summary>
		/// <param name="state">Not used</param>
		private void ProcessQueue(object? state)
		{
			while(true)
			{
				T data;
				var result=Dequeue(out data);
				if(result==DequeueResult.StopProcessing) break;

				Process(data);
			}

			lock(m_ActiveConsumerSyncRoot)
			{
				m_ActiveConsumers--;
				Monitor.Pulse(m_ActiveConsumerSyncRoot);
			}
		}

		/// <summary>
		/// Runs an individual worker.
		/// This method is guaranteed not to throw an exception
		/// </summary>
		/// <param name="data">The data to process</param>
		protected abstract void Process(T data);



		enum DequeueResult
		{
			/// <summary>
			/// Process the item
			/// </summary>
			Process,
			
			/// <summary>
			/// Stop processing immediately
			/// </summary>
			StopProcessing
		}
	}
}
