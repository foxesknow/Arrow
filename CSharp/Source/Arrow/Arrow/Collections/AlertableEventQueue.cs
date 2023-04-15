using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Collections
{
	/// <summary>
	/// A thread safe queue that sets an event when data is available.
	/// In a multiple-producer/single-consumer model you can use Deque to retrieve items
	/// In a multiple-producer/multiple-consumer model you should use TryDeque to retrieve items
	/// </summary>
	/// <typeparam name="T">The type of data to hold in the queue</typeparam>
	public sealed class AlertableEventQueue<T> : IDisposable
	{
        private ManualResetEvent m_Available = new ManualResetEvent(false);

        private readonly object m_SyncRoot = new object();
        private Queue<T> m_Data = new Queue<T>();

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public AlertableEventQueue()
        {
        }

        /// <summary>
        /// Initialize the instance
        /// </summary>
        /// <param name="collection">A sequence of items to add to the queue</param>
        public AlertableEventQueue(IEnumerable<T> collection)
        {
            if(collection == null) throw new ArgumentNullException("collection");

            foreach(T data in collection)
            {
                m_Data.Enqueue(data);
            }

            if(m_Data.Count != 0) m_Available.Set();
        }

        /// <summary>
        /// Adds an item to the back of the queue
        /// </summary>
        /// <param name="data">The item to add</param>
        public void Enqueue(T data)
        {
            lock(m_SyncRoot)
            {
                m_Data.Enqueue(data);
                if(m_Data.Count == 1) m_Available.Set();
            }
        }

        /// <summary>
        /// Removes an item from the front of the queue
        /// </summary>
        /// <returns>The item at the front</returns>
        /// <exception cref="System.InvalidOperationException">The queue is empty</exception>
        public T Dequeue()
        {
            lock(m_SyncRoot)
            {
                T data = m_Data.Dequeue();
                if(m_Data.Count == 0) m_Available.Reset();
                return data;
            }
        }

        /// <summary>
        /// Attempts to remove an item from the queue
        /// </summary>
        /// <param name="data">On success the item at the front, otherwise the default value for T</param>
        /// <returns>true is an item was removed, otherwise false</returns>
        public bool TryDequeue([MaybeNullWhen(false)] out T data)
        {
            bool gotData = false;

            lock(m_SyncRoot)
            {
                if(m_Data.Count != 0)
                {
                    data = Dequeue();
                    gotData = true;
                }
                else
                {
                    data = default(T);
                }
            }

            return gotData;
        }

        /// <summary>
        /// Releases all resources held by the queue
        /// </summary>
        public void Close()
        {
            lock(m_SyncRoot)
            {
                m_Available.Close();
                m_Data.Clear();
            }
        }

        /// <summary>
        /// A handle that can be waited on until data is available
        /// </summary>
        public WaitHandle AvailableHandle
		{
			get{return m_Available;}
		}
		
		/// <summary>
		/// Returns the number of items in the queue
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_Data.Count;
				}
			}
		}

		/// <summary>
		/// Releases any resouces
		/// </summary>
		public void Dispose()
		{
			Close();
		}
	}
}
