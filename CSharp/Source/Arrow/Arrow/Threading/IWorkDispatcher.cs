using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading
{
	/// <summary>
	/// Defines the behavior for dispatching a work item to a thread
	/// </summary>
	public interface IWorkDispatcher
	{
		/// <summary>
		/// Adds a request to the work dispatcher
		/// </summary>
		/// <param name="waitCallback">The delegate that will be called</param>
		/// <returns>true if the request was queued, false otherwise</returns>
		bool QueueUserWorkItem(WaitCallback waitCallback);
		
		/// <summary>
		/// Adds a request to the work queue
		/// </summary>
		/// <param name="waitCallback">The delegate that will be called</param>
		/// <param name="state">Any additional state information for the request</param>
		/// <returns>true if the request was queued, false otherwise</returns>
		bool QueueUserWorkItem(WaitCallback waitCallback, object? state);
	}
}
