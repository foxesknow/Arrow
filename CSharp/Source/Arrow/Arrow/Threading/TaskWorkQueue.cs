using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading
{
	/// <summary>
	/// A work queue that returns Task instances
	/// </summary>
	public class TaskWorkQueue : IDisposable
	{
		private readonly ActionWorkQueue m_WorkQueue;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		public TaskWorkQueue() : this(null)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">The work dispatcher to use</param>
		public TaskWorkQueue(IWorkDispatcher dispatcher)
		{
			m_WorkQueue=new ActionWorkQueue(dispatcher);
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

			var source=new TaskCompletionSource<T>();

			m_WorkQueue.Enqueue(()=>
			{
				try
				{
					source.SetResult(function());
				}
				catch(Exception e)
				{
					source.SetException(e);
				}
			});

			return source.Task;
		}

		/// <summary>
		/// Schedules an action for running on the work queue
		/// </summary>
		/// <param name="action">The action to execute</param>
		/// <returns>A task that allows you to synchronize on the execution of the function</returns>
		public Task Enqueue(Action action)
		{
			if(action==null) throw new ArgumentException("action");

			var source=new TaskCompletionSource<bool>();

			m_WorkQueue.Enqueue(()=>
			{
				try
				{
					action();
					source.SetResult(true);
				}
				catch(Exception e)
				{
					source.SetException(e);
				}
			});

			return source.Task;
		}

		/// <summary>
		/// Returns the number of items in the queue
		/// </summary>
		public int Count
		{
			get{return m_WorkQueue.Count;}
		}

		/// <summary>
		/// Waits for any outstanding work to be processed and shuts the queue down
		/// </summary>
		public void Close()
		{
			m_WorkQueue.Close();
		}

		public void Dispose()
		{
			m_WorkQueue.Dispose();
		}
	}
}
