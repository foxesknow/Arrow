using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading
{
	/// <summary>
	/// Implements a queue that signals a monitor when items
	/// are added, allowing you to wait for items to become available.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class MonitorQueue<T> : IEnumerable<T>
	{
		private readonly object m_SyncRoot=new object();
		
		private readonly Queue<T> m_Queue;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		public MonitorQueue()
		{
			m_Queue=new Queue<T>();
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="collection">An initial set of values to add to the queue</param>
		public MonitorQueue(IEnumerable<T> collection)
		{
			if(collection==null) throw new ArgumentNullException("collection");
			m_Queue=new Queue<T>(collection);
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="capacity">The initial capacity of the queue</param>
		public MonitorQueue(int capacity)
		{
			m_Queue=new Queue<T>(capacity);
		}

		/// <summary>
		/// Adds an item to the queue and signals any waiting threads
		/// </summary>
		/// <param name="item">The item to add</param>
		public void Enqueue(T item)
		{
			lock(m_SyncRoot)
			{
				m_Queue.Enqueue(item);
				Monitor.Pulse(m_SyncRoot);
			}
		}

		/// <summary>
		/// Adds a sequence of items to the queue and signals any waiting threads
		/// </summary>
		/// <param name="collection">The items to add</param>
		public void Enqueue(IEnumerable<T> collection)
		{
			if(collection==null) throw new ArgumentNullException("collection");

			lock(m_SyncRoot)
			{
                int itemsAdded=0;

				foreach(T item in collection)
				{
					m_Queue.Enqueue(item);
                    itemsAdded++;
				}

                if(itemsAdded!=0)
                {
                    if(itemsAdded==1)
                    {
                        Monitor.Pulse(m_SyncRoot);
                    }
                    else
                    {
				        Monitor.PulseAll(m_SyncRoot);
                    }
                }
			}
		}

		/// <summary>
		/// Attempts to get an item from the queue without waiting
		/// </summary>
		/// <param name="item">On success, the item from the front of the queue</param>
		/// <returns>true if an item was taken from the queue, false otherwise</returns>
		public bool TryDequeue([MaybeNullWhen(false)] out T item)
		{
			bool gotItem=false;

			lock(m_SyncRoot)
			{
				if(m_Queue.Count!=0)
				{
					item=m_Queue.Dequeue();
					gotItem=true;
				}
				else
				{
					item=default;
				}
			}

			return gotItem;
		}

		/// <summary>
		/// Attempts to get an item from the queue, waiting if necessary
		/// </summary>
		/// <param name="timespan">How long to wait if nothing is on the queue</param>
		/// <param name="item">On success, the item from the front of the queue</param>
		/// <returns>true if an item was taken from the queue, false otherwise</returns>
		public bool TryDequeue(TimeSpan timespan, [MaybeNullWhen(false)] out T item)
		{
			bool gotItem=false;

			lock(m_SyncRoot)
			{
				if(m_Queue.Count==0)
				{
					Monitor.Wait(m_SyncRoot,timespan);

					if(m_Queue.Count!=0)
					{
						item=m_Queue.Dequeue();
						gotItem=true;
					}
					else
					{
						item=default;
					}
				}
				else
				{
					item=m_Queue.Dequeue();
					gotItem=true;
				}
			}

			return gotItem;
		}

		/// <summary>
		/// Removes an item from the queue, waiting, if necessary, for an item be become available
		/// </summary>
		/// <returns>The item at the front of the queue</returns>
		public T DequeueWithWait()
		{
			lock(m_SyncRoot)
			{
				while(m_Queue.Count==0)
				{
					Monitor.Wait(m_SyncRoot);
				}

				return m_Queue.Dequeue();
			}
		}

		/// <summary>
		/// Removes an item from the queue
		/// </summary>
		/// <returns>The item at the front of the queue</returns>
		public T Dequeue()
		{
			lock(m_SyncRoot)
			{
				return m_Queue.Dequeue();
			}
		}

		/// <summary>
		/// Attempts to return the item at the front of the queue, without removing it
		/// </summary>
		/// <param name="item">On success the item at the front of the queue</param>
		/// <returns>true if there was an item that was peeked, false otherwise</returns>
		public bool TryPeek([MaybeNullWhen(false)]out T item)
		{
			bool peeked=false;

			lock(m_SyncRoot)
			{
				if(m_Queue.Count!=0)
				{
					item=m_Queue.Peek();
					peeked=true;
				}
				else
				{
					item=default;
				}
			}

			return peeked;
		}

		/// <summary>
		/// Attempts to return the item at the front of the queue, without removing it
		/// </summary>
		/// <param name="timespan">How long to wait if nothing is on the queue</param>
		/// <param name="item">On success the item at the front of the queue</param>
		/// <returns>true if an item was peeked, false otherwise</returns>
		public bool TryPeek(TimeSpan timespan, [MaybeNullWhen(false)]out T item)
		{
			bool peeked=false;

			lock(m_SyncRoot)
			{
				if(m_Queue.Count==0)
				{
					Monitor.Wait(m_SyncRoot,timespan);

					if(m_Queue.Count!=0)
					{
						item=m_Queue.Peek();
						peeked=true;
					}
					else
					{
						item=default;
					}
				}
				else
				{
					item=m_Queue.Peek();
					peeked=true;
				}
			}

			return peeked;
		}

		/// <summary>
		/// Returns the item at the front of the queue
		/// </summary>
		/// <returns>The item at the front</returns>
		public T Peek()
		{
			lock(m_SyncRoot)
			{
				return m_Queue.Peek();
			}
		}

		/// <summary>
		/// The synchronization object that is signalled when data is available.
		/// A consumer can wait on this object via Monitor.Wait in order to be
		/// notified when data is in the queue
		/// </summary>
		public object SyncRoot
		{
			get{return m_SyncRoot;}
		}

		/// <summary>
		/// The number of items in the queue
		/// </summary>
		public int Count
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_Queue.Count;
				}
			}
		}

		/// <summary>
		/// Removes all items from the queue
		/// </summary>
		public void Clear()
		{
			lock(m_SyncRoot)
			{
				m_Queue.Clear();
			}
		}

		#region IEnumerable<T> Members

		/// <summary>
		/// Returns an enumerator for the values in the queue.
		/// The queue is locked whilst the enumerator is active.
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<T> GetEnumerator()
		{
			lock(m_SyncRoot)
			{
				foreach(T item in m_Queue)
				{
					yield return item;
				}
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
