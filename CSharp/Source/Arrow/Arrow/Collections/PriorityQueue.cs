using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable

namespace Arrow.Collections
{
	/// <summary>
	/// A priority queue where, by default, the value with the highest priority is returned.
	/// A heap is used to manage the queue, and therefore the queue is not stable for items
	/// with the same priority
	/// </summary>
	/// <typeparam name="P">The type of the priority</typeparam>
	/// <typeparam name="V">The type of the value</typeparam>
	[Serializable]
	public sealed class PriorityQueue<P,V> : IPriorityQueue<P,V>
	{
		private readonly Heap<KeyValuePair<P,V>> m_Heap;

        /// <summary>
        /// Initializes the instance with a default priority comparer
        /// </summary>
        public PriorityQueue() : this(null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="priorityComparer">The priority comparer to use</param>
        public PriorityQueue(IComparer<P> priorityComparer)
        {
            if(priorityComparer == null) priorityComparer = Comparer<P>.Default;

            Comparer comparer = new Comparer { OuterPriority = priorityComparer };
            m_Heap = new Heap<KeyValuePair<P, V>>(comparer);
        }

        /// <summary>
        /// Adds a value to the queue
        /// </summary>
        /// <param name="priority">The priority of the value</param>
        /// <param name="value">The value to add</param>
        public void Enqueue(P priority, V value)
        {
            m_Heap.Add(new KeyValuePair<P, V>(priority, value));
        }

        /// <summary>
        /// Dequeues a value
        /// </summary>
        /// <returns>The value at the front of the queue</returns>
        public V Dequeue()
        {
            if(m_Heap.Count == 0) throw new InvalidOperationException("queue is empty");

            return m_Heap.Extract().Value;
        }

        /// <summary>
        /// Attempts to dequeue a value
        /// </summary>
        /// <param name="priority">On success the priority of the value at the front of the queue</param>
        /// <param name="value">On success the value at the front of the queue</param>
        /// <returns>true if a value was dequeued, false otherwise</returns>
        public bool TryDequeue(out P priority, out V value)
        {
            bool success = false;

            if(m_Heap.Count != 0)
            {
                var front = m_Heap.Extract();

                priority = front.Key;
                value = front.Value;

                success = true;
            }
            else
            {
                priority = default(P);
                value = default(V);
                success = false;
            }
            return success;
        }

        /// <summary>
        /// Attempts to return the item at the front of the queue
        /// </summary>
        /// <returns>The item at the front of the queue</returns>
        public V Peek()
        {
            if(m_Heap.Count == 0) throw new InvalidOperationException("queue is empty");

            return m_Heap[0].Value;
        }

        /// <summary>
        /// Returns the number of items in the queue
        /// </summary>
        public int Count
		{
			get{return m_Heap.Count;}
		}

        /// <summary>
        /// Removes all items from the queue
        /// </summary>
        public void Clear()
        {
            m_Heap.Clear();
        }

        /// <summary>
        /// Allows for enumeration over the queue.
        /// NOTE: The order that items are returned is not guaranteed
        /// </summary>
        /// <returns>An enumerator</returns>
        public IEnumerator<KeyValuePair<P, V>> GetEnumerator()
        {
            return m_Heap.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class Comparer : IComparer<KeyValuePair<P, V>>
        {
            public IComparer<P> OuterPriority = default!;

            public int Compare(KeyValuePair<P, V> x, KeyValuePair<P, V> y)
            {
                return OuterPriority.Compare(x.Key, y.Key);
            }
        }
    }
}
