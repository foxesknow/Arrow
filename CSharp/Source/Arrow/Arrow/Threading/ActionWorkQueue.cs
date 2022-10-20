using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Threading
{
	/// <summary>
	/// Implementation of a work queue that allows an arbitary action to be executes asynchronously
	/// </summary>
	public sealed class ActionWorkQueue : WorkQueue<Action>
	{
		/// <summary>
		/// Initializes the instance using the default work item dispatcher
		/// </summary>
		public ActionWorkQueue() : base(null)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		public ActionWorkQueue(IWorkDispatcher? dispatcher) : base(dispatcher,8)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		/// <param name="initialCapacity">The initial capacity for the queue</param>
		public ActionWorkQueue(IWorkDispatcher? dispatcher, int initialCapacity) : base(dispatcher,initialCapacity)
		{
		}
	
		/// <summary>
		/// Executes an action
		/// </summary>
		/// <param name="action">The action to execute</param>
		protected override void Process(Action action)
		{
			try
			{
				action();
			}
			catch
			{
				// Ignore it
			}
		}
	}
}
