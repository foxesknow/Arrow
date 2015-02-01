using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Arrow.Threading
{
	/// <summary>
	/// A work dispatcher that launces a new thread for each work item
	/// </summary>
	public class LaunchThreadWorkDispatcher : IWorkDispatcher, IDisposable
	{
		private readonly OutstandingEvent m_OutstandingEvent=new OutstandingEvent();

		/// <summary>
		/// Returns the number of threads currently active
		/// </summary>
		public int ActiveThreads
		{
			get{return m_OutstandingEvent.Count;}
		}

		/// <summary>
		/// Returns an outstanding event that allows you to wait for various scenarios
		/// </summary>
		public OutstandingEvent OutstandingWork
		{
			get{return m_OutstandingEvent;}
		}

		/// <summary>
		/// Queues work for execution
		/// </summary>
		/// <param name="waitCallback">The callback to execute</param>
		/// <returns>true if the work was queued, otherwise false</returns>
		public bool QueueUserWorkItem(WaitCallback waitCallback)
		{
			return QueueUserWorkItem(waitCallback,null);
		}

		/// <summary>
		/// Queues work for execution
		/// </summary>
		/// <param name="waitCallback">The callback to execute</param>
		/// <param name="state">Any additional state to pass to the callback</param>
		/// <returns>true if the work was queued, otherwise false</returns>
		public bool QueueUserWorkItem(WaitCallback waitCallback, object state)
		{
			if(waitCallback==null) throw new ArgumentNullException("waitCallback");

			var thread=new Thread(()=>
			{
				try
				{
					waitCallback(state);
				}
				finally
				{
					m_OutstandingEvent.Decrease();
				}
			});

			// NOTE: We increase the count here as the thread may not start as soon
			// as we call Thread.Start() but we need to record the work item
			m_OutstandingEvent.Increase();
			thread.Start();

			return true;
		}

		/// <summary>
		/// Waits for all outstanding work to complete and then disposes the object
		/// </summary>
		public void Dispose()
		{
			// Wait for everything to finish
			m_OutstandingEvent.NothingOutstandingHandle.WaitOne();
			m_OutstandingEvent.Dispose();
		}
	}
}
