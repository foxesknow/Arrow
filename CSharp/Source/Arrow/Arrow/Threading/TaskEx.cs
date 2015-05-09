using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading
{
	/// <summary>
	/// Useful Task related methods that are not on Task
	/// </summary>
	public static class TaskEx
	{
		/// <summary>
		/// Creates a failed task
		/// </summary>
		/// <param name="exception">The reason the task failed</param>
		/// <returns>A failed task</returns>
		public static Task FromException(Exception exception)
		{
			if(exception==null) throw new ArgumentNullException("exception");

			var source=new TaskCompletionSource<bool>();
			source.SetException(exception);

			return source.Task;
		}

		/// <summary>
		/// Creates a failed task
		/// </summary>
		/// <typeparam name="T">The task type</typeparam>
		/// <param name="exception">The reason the task failed</param>
		/// <returns>A failed task</returns>
		public static Task<T> FromException<T>(Exception exception)
		{
			if(exception==null) throw new ArgumentNullException("exception");

			var source=new TaskCompletionSource<T>();
			source.SetException(exception);

			return source.Task;
		}
	}
}
