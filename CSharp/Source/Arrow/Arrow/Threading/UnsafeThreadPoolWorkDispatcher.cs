﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading
{
	/// <summary>
	/// Dispatches work items to the .NET ThreadPool class using UnsafeQueueUserWorkItem
	/// </summary>
	class UnsafeThreadPoolWorkDispatcher : IWorkDispatcher
	{
		/// <summary>
		/// Adds a request to the work dispatcher
		/// </summary>
		/// <param name="waitCallback">The delegate that will be called</param>
		/// <returns>true if the request was queued, false otherwise</returns>
		public bool QueueUserWorkItem(WaitCallback waitCallback)
		{
			return ThreadPool.UnsafeQueueUserWorkItem(waitCallback,null);
		}

		/// <summary>
		/// Adds a request to the work queue
		/// </summary>
		/// <param name="waitCallback">The delegate that will be called</param>
		/// <param name="state">Any additional state information for the request</param>
		/// <returns>true if the request was queued, false otherwise</returns>
		public bool QueueUserWorkItem(WaitCallback waitCallback, object? state)
		{
			return ThreadPool.UnsafeQueueUserWorkItem(waitCallback,state);
		}
	}
}
