using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable

namespace Arrow.Collections
{
	/// <summary>
	/// A priority queue where, by default, the value with the highest priority is returned.
	/// The queue is stable for items added with the same priority
	/// </summary>
	/// <typeparam name="P">The type of the priority</typeparam>
	/// <typeparam name="V">The type of the value</typeparam>
	[Serializable]
	public sealed class StablePriorityQueue<P,V> : IPriorityQueue<P,V> where P : notnull
	{
        private SortedList<P, Queue<V>> m_Data;

        private int m_Count;

        /// <summary>
        /// Initializes the instance with a default priority comparer
        /// </summary>
        public StablePriorityQueue() : this(null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="priorityComparer">The priority comparer to use</param>
        public StablePriorityQueue(IComparer<P> priorityComparer)
        {
            if(priorityComparer == null) priorityComparer = Comparer<P>.Default;

            priorityComparer = new MaxComparer { Outer = priorityComparer };
            m_Data = new SortedList<P, Queue<V>>(priorityComparer);
        }

        /// <summary>
        /// Removes all items from the queue
        /// </summary>
        public void Clear()
        {
            m_Data.Clear();
            m_Count = 0;
        }

        /// <summary>
        /// Adds a value to the queue
        /// </summary>
        /// <param name="priority">The priority of the value</param>
        /// <param name="value">The value to add</param>
        public void Enqueue(P priority, V value)
        {
            if(m_Data.TryGetValue(priority, out var queue) == false)
            {
                queue = new Queue<V>();
                m_Data.Add(priority, queue);
            }

            queue.Enqueue(value);
            m_Count++;
        }

        /// <summary>
        /// Dequeues a value
        /// </summary>
        /// <returns>The value at the front of the queue</returns>
        public V Dequeue()
        {
            if(m_Count == 0) throw new InvalidOperationException("queue is empty");

            P priority = m_Data.Keys[0];
            Queue<V> queue = m_Data[priority];

            V value = queue.Dequeue();

            // Remove any empty queue
            if(queue.Count == 0) m_Data.RemoveAt(0);

            m_Count--;

            return value;
        }

        /// <summary>
        /// Attempts to dequeue a value
        /// </summary>
        /// <param name="priority">On success the priority of the value at the front of the queue</param>
        /// <param name="value">On success the value at the front of the queue</param>
        /// <returns>true if a value was dequeued, false otherwise</returns>
        public bool TryDequeue(out P priority, out V value)
        {
            if(m_Count == 0)
            {
                priority = default!;
                value = default!;
                return false;
            }

            priority = m_Data.Keys[0];
            Queue<V> queue = m_Data[priority];

            value = queue.Dequeue();

            // Remove any empty queue
            if(queue.Count == 0) m_Data.RemoveAt(0);

            m_Count--;

            return true;
        }

        /// <summary>
        /// Attempts to return the item at the front of the queue
        /// </summary>
        /// <returns>The item at the front of the queue</returns>
        public V Peek()
        {
            if(m_Count == 0) throw new InvalidOperationException("queue is empty");

            P priority = m_Data.Keys[0];
            Queue<V> queue = m_Data[priority];

            return queue.Peek();
        }

        /// <summary>
        /// Returns the number of items in the queue
        /// </summary>
        public int Count
		{
			get{return m_Count;}
		}

		/// <summary>
		/// Allows for enumeration over the queue
		/// NOTE: Items are returned from highest to lowest priority in the 
		/// order they were enqueued for that priority
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<KeyValuePair<P,V>> GetEnumerator()
        {
            foreach(var pair in m_Data)
            {
                P priority = pair.Key;

                foreach(V value in pair.Value)
                {
                    yield return new KeyValuePair<P, V>(priority, value);
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class MaxComparer : IComparer<P>
		{
			public IComparer<P> Outer;

			public int Compare(P x, P y)
			{
                // NOTE: we invert the result so that the "largest" comes first
                return  Outer!.Compare(y, x);
            }
		}
	}
}
