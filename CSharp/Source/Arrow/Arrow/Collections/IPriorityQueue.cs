using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections
{
	/// <summary>
	/// Defines the behaviour of a priority queue
	/// </summary>
	/// <typeparam name="P">The priority type</typeparam>
	/// <typeparam name="V">The value type</typeparam>
	public interface IPriorityQueue<P,V> : IEnumerable<KeyValuePair<P,V>>
	{
		/// <summary>
		/// Enqueues a value
		/// </summary>
		/// <param name="priority">The priority of the value</param>
		/// <param name="value">The value to enqueue</param>
		void Enqueue(P priority, V value);
		
		/// <summary>
		/// Dequeues a value
		/// </summary>
		/// <returns>The value at the front of the queue</returns>
		V Dequeue();
		
		/// <summary>
		/// Attempts to dequeue a value
		/// </summary>
		/// <param name="priority">On success the priority of the value at the front of the queue</param>
		/// <param name="value">On success the value at the front of the queue</param>
		/// <returns>true if a value was dequeued, false otherwise</returns>
		bool TryDequeue(out P priority, out V value);
		
		/// <summary>
		/// Attempts to return the item at the front of the queue
		/// </summary>
		/// <returns>The item at the front of the queue</returns>
		V Peek();
		
		/// <summary>
		/// Returns the number of items in the queue
		/// </summary>
		int Count{get;}
		
		/// <summary>
		/// Removes all items from the queue
		/// </summary>
		void Clear();
	}
}
