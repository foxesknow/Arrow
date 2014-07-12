using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Threading
{
	/// <summary>
	/// Implementation of a multi consumer work queue that allows an arbitary action to be executes asynchronously
	/// </summary>
	public class ActionMultiConsumerWorkQueue : MultiConsumerWorkQueue<Action>
	{
		/// <summary>
		/// Initializes the instance using the default work item dispatcher
		/// </summary>
		public ActionMultiConsumerWorkQueue() : base()
		{
		}

		/// <summary>
		/// Initializes the instance using the default work item dispatcher
		/// <param name="numberOfConsumers">The number of consumer threads for the work</param>
		/// </summary>
		public ActionMultiConsumerWorkQueue(int numberOfConsumers) : base(numberOfConsumers)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dispatcher">A dispatcher that will place work into a thread pool. If null the default dispatcher is used</param>
		/// <param name="numberOfConsumers">The number of work consumers</param>
		public ActionMultiConsumerWorkQueue(IWorkDispatcher dispatcher, int numberOfConsumers) : base(dispatcher,numberOfConsumers)
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
