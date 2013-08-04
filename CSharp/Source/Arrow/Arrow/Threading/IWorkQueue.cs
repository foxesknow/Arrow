using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Threading
{
	/// <summary>
	/// Defines a work queue that lets work be moved from one or more producers to a consumer
	/// </summary>
	/// <typeparam name="T">The type of work to queue</typeparam>
	public interface IWorkQueue<T> : IDisposable
	{
		/// <summary>
		/// Adds the unit of work to the queue
		/// </summary>
		/// <param name="work">The work to queue for processing</param>
		void Enqueue(T work);
	}
}
