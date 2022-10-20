using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

#nullable disable

namespace Arrow.Collections
{
	/// <summary>
	/// Implements a queue that holds a maximum number of items.
	/// When the maximum size is exceeded the oldest item is removed
	/// </summary>
	/// <typeparam name="T">The type of item to store in the queue</typeparam>
	[Serializable]
	public class RollingQueue<T> : IEnumerable<T>, System.Collections.ICollection
	{
		private int m_MaxCount;
		private Queue<T> m_Queue;
		
		private object m_SyncRoot;
	
		/// <summary>
		/// Initializes a queue with the specified maximum number of items
		/// </summary>
		/// <param name="maxCount">The maximum number of items to hold</param>
		/// <exception cref="System.ArgumentException">maxCount is less than 1</exception>
		public RollingQueue(int maxCount)
		{
			if(maxCount<1) throw new ArgumentException("maxCount");
			
			m_MaxCount=maxCount;
			m_Queue=new Queue<T>(maxCount);
		}
		
		/// <summary>
		/// Adds a new item to the end of the queue.
		/// If the number of items in the queue is MaxSize then the oldest item is removed
		/// </summary>
		/// <param name="item">The item to add to the queue</param>
		public void Enqueue(T item)
		{
			if(m_Queue.Count==m_MaxCount)
			{
				m_Queue.Dequeue();
			}
			
			m_Queue.Enqueue(item);
		}
		
		/// <summary>
		/// Removes the item from the beginning of the queue
		/// </summary>
		/// <returns>The item at the beginning of the queue</returns>
		/// <exception cref="System.InvalidOperationException">The queue is empty</exception>
		public T Dequeue()
		{
			return m_Queue.Dequeue();
		}
		
		/// <summary>
		/// Returns the item at the beginning of the queue without removing it
		/// </summary>
		/// <returns>The item at the beginning of the queue</returns>
		/// <exception cref="System.InvalidOperationException">The queue is empty</exception>
		public T Peek()
		{
			return m_Queue.Peek();
		}
		
		/// <summary>
		/// The number of items in the queue
		/// </summary>
		/// <value>The number of items in the queue</value>
		public int Count
		{
			get{return m_Queue.Count;}
		}
		
		/// <summary>
		/// Determines if an item is in the queue
		/// </summary>
		/// <param name="item">The item to search for</param>
		/// <returns>true if item is in the queue, false otherwise</returns>
		public bool Contains(T item)
		{
			return m_Queue.Contains(item);
		}
		
		/// <summary>
		/// The maximum number of items allowed in the queue
		/// </summary>
		/// <value>The maximun number of items in the queue</value>
		public int MaxCount
		{
			get{return m_MaxCount;}
		}
		
		/// <summary>
		/// Removes all items from the queue
		/// </summary>
		public void Clear()
		{
			m_Queue.Clear();
		}

		#region IEnumerable<T> Members

		/// <summary>
		/// Returns an enumerator to the items in the queue
		/// </summary>
		/// <returns>An enumerator to the items in the queue</returns>
		public IEnumerator<T> GetEnumerator()
		{
			return m_Queue.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		/// <summary>
		/// Returns an enumerator to the items in the queue
		/// </summary>
		/// <returns>An enumerator to the items in the queue</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		#endregion

		#region ICollection Members

		/// <summary>
		/// Copies the entire queue to an array
		/// </summary>
		/// <param name="array"></param>The one-dimensional array that will receive the elements
		/// <param name="index">The zero-bases index in array at which copying will begin</param>
		/// <exception cref="System.ArgumentNullException">array is null</exception>
		/// <exception cref="System.ArgumentException">arrayIndex is less that 0</exception>
		public void CopyTo(Array array, int index)
		{
			(m_Queue as System.Collections.ICollection).CopyTo(array,index);
		}

		/// <summary>
		/// Indicates if the queue operations are synchronized
		/// </summary>
		/// <value>Always false</value>
		public bool IsSynchronized
		{
			get{return false;}
		}

		/// <summary>
		/// Returns an object that callers can use to synchronize access to the queue
		/// </summary>
		/// <value>An object to synchronize on</value>
		public object SyncRoot
		{
			get
			{
				if(m_SyncRoot==null)
				{
					Interlocked.CompareExchange(ref m_SyncRoot,new object(),null);
				}
				
				return m_SyncRoot;
			}
		}

		#endregion
	}
}
