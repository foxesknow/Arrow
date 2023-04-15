using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

#nullable disable

namespace Arrow.Collections
{
	/// <summary>
	/// Represents a mapping of keys to one or more values
	/// </summary>
	/// <typeparam name="TKey">The type of the keys in the dictionary</typeparam>
	/// <typeparam name="TValue">The type of the values in the dictionary</typeparam>
	[Serializable]
	public sealed class MultiDictionary<TKey,TValue> : IDictionary<TKey,TValue> where TKey : notnull
	{
        private Dictionary<TKey, IList<TValue>> m_Data = new Dictionary<TKey, IList<TValue>>();

        /// <summary>
        /// Initializes the instance with the default equality comparer
        /// </summary>
        public MultiDictionary() : this(null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="comparer">The comparer to use when comparing keys, or null to use the default</param>
        public MultiDictionary(IEqualityComparer<TKey> comparer)
        {
            m_Data = new Dictionary<TKey, IList<TValue>>(comparer);
        }

        /// <summary>
        /// Adds a new item to the dictionary
        /// </summary>
        /// <param name="key">The key for pair</param>
        /// <param name="value">The value for the pair</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        public void Add(TKey key, TValue value)
        {
            if(key == null) throw new ArgumentNullException("key");

            IList<TValue> values = GetOrCreate(key);
            values.Add(value);
        }

        /// <summary>
        /// Adds a sequence of values to a key
        /// </summary>
        /// <param name="key">The key for the pair</param>
        /// <param name="values">A sequence of values to make key/value pairs from</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.ArgumentNullException">values is null</exception>
        public void Add(TKey key, params TValue[] values)
        {
            if(key == null) throw new ArgumentNullException("key");
            if(values == null) throw new ArgumentNullException("values");

            if(values != null)
            {
                IList<TValue> existingValues = GetOrCreate(key);
                foreach(TValue value in values)
                {
                    existingValues.Add(value);
                }
            }
        }

        /// <summary>
        /// Adds a sequence of values to a key
        /// </summary>
        /// <param name="key">The key for the pair</param>
        /// <param name="values">A sequence of values to make key/value pairs from</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.ArgumentNullException">values is null</exception>
        public void Add(TKey key, IList<TValue> values)
        {
            if(key == null) throw new ArgumentNullException("key");
            if(values == null) throw new ArgumentNullException("values");

            IList<TValue> existingValues = GetOrCreate(key);
            foreach(TValue value in values)
            {
                existingValues.Add(value);
            }
        }

        /// <summary>
        /// Determines if a key is present
        /// </summary>
        /// <param name="key">The key to look for</param>
        /// <returns>true if the key exists, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        public bool ContainsKey(TKey key)
        {
            if(key == null) throw new ArgumentNullException("key");

            return m_Data.ContainsKey(key);
        }

        /// <summary>
        /// Determines if a value is present
        /// </summary>
        /// <param name="value">The value to search for</param>
        /// <returns>true if the exists, false otherwise</returns>
        public bool ContainsValue(TValue value)
        {
            bool found = false;

            foreach(KeyValuePair<TKey, IList<TValue>> pair in m_Data)
            {
                found = pair.Value.Contains(value);
                if(found) break;
            }

            return found;
        }

        /// <summary>
        /// Returns all the keys in the dictionary
        /// </summary>
        public ICollection<TKey> Keys
		{
			get{return m_Data.Keys;}
		}

        /// <summary>
        /// Attempts to remove the key and all its values from the dictionary
        /// </summary>
        /// <param name="key">The key to remove</param>
        /// <returns>true if the key was removed, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        public bool Remove(TKey key)
        {
            if(key == null) throw new ArgumentNullException("key");
            return m_Data.Remove(key);
        }

        /// <summary>
        /// Returns the first item that maps to a key
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <param name="value">On success the first value in the sequence</param>
        /// <returns>true if there is a value that maps to key, false otherwise</returns>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if(key == null) throw new ArgumentNullException("key");

            bool found = false;

            if(m_Data.TryGetValue(key, out var values))
            {
                if(values.Count != 0)
                {
                    value = values[0];
                    found = true;
                }
                else
                {
                    value = default!;
                }
            }
            else
            {
                value = default!;
            }

            return found;
        }

        /// <summary>
        /// Attempts to get the sequence of values mapped to a key
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <param name="values">On success the sequence of values that map to "key"</param>
        /// <returns>true if there is a sequence of values, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        public bool TryGetValue(TKey key, out IList<TValue> values)
        {
            return m_Data.TryGetValue(key, out values);
        }

        /// <summary>
        /// Returns a sequence of all values stored in the dictionary
        /// </summary>
        public ICollection<TValue> Values
        {
            get
            {
                List<TValue> values = new List<TValue>();

                foreach(KeyValuePair<TKey, IList<TValue>> pair in m_Data)
                {
                    values.AddRange(pair.Value);
                }

                return values;
            }
        }

        /// <summary>
        /// Returns a sequence of all values for a key
        /// </summary>
        /// <param name="key">The key to lookup</param>
        /// <returns>A sequence of values, if successful</returns>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">the key does not exist</exception>
        public IList<TValue> ValuesFor(TKey key)
        {
            if(key == null) throw new ArgumentNullException("key");

            if(m_Data.TryGetValue(key, out var values) == false)
            {
                throw new KeyNotFoundException();
            }

            return values;
        }

        /// <summary>
        /// Gets or sets the first value for a key
        /// </summary>
        /// <param name="key">The key to get or set</param>
        /// <returns>The value for the key</returns>
        public TValue this[TKey key]
        {
            get
            {
                if(key == null) throw new ArgumentNullException("key");

                if(m_Data.TryGetValue(key, out var values) && values.Count != 0)
                {
                    return values[0];
                }

                throw new KeyNotFoundException();

            }
            set
            {
                if(key == null) throw new ArgumentNullException("key");

                IList<TValue> values = GetOrCreate(key);
                if(values.Count == 0)
                {
                    values.Add(value);
                }
                else
                {
                    values[0] = value;
                }
            }
        }

        /// <summary>
        /// Adds a pair to the dictionary
        /// </summary>
        /// <param name="item">The pair to add</param>
        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        /// <summary>
        /// Removes all keys and values from the dictionary
        /// </summary>
        public void Clear()
        {
            m_Data.Clear();
        }

        /// <summary>
        /// Removes all values for a key, so that the key maps to an empty sequence
        /// </summary>
        /// <param name="key">The key to clear</param>
        /// <exception cref="System.ArgumentNullException">key is null</exception>
        public void ClearValues(TKey key)
        {
            if(key == null) throw new ArgumentNullException("key");

            if(m_Data.TryGetValue(key, out var values))
            {
                values.Clear();
            }
        }

        /// <summary>
        /// Determines if a pair is present in the dictionary
        /// </summary>
        /// <param name="item">The pair to search for</param>
        /// <returns>rue if the pair exists, false otherwise</returns>
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            bool found = false;

            if(m_Data.TryGetValue(item.Key, out var values))
            {
                found = values.Contains(item.Value);
            }

            return found;
        }

        /// <summary>
        /// Copies all dictionary elements into an array starting at the specified index
        /// </summary>
        /// <param name="array"></param>The one-dimensional array that will receive the elements
        /// <param name="arrayIndex">The zero-bases index in array at which copying will begin</param>
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            foreach(KeyValuePair<TKey, TValue> pair in this)
            {
                array[arrayIndex++] = pair;
            }
        }

        /// <summary>
        /// Returns the number of values in the dictionary
        /// </summary>
        public int Count
		{
			get
			{
                int count = 0;
                foreach(KeyValuePair<TKey, IList<TValue>> pair in m_Data)
                {
                    count += pair.Value.Count;
                }

                return count;
            }
		}
		
		/// <summary>
		/// Returns the number of keys in the dictionary
		/// </summary>
		public int KeyCount
		{
			get{return m_Data.Count;}
		}

		/// <summary>
		/// Returns false
		/// </summary>
		public bool IsReadOnly
		{
			get{return false;}
		}

		/// <summary>
		/// Attempts to remove an element from the dictionary.
		/// </summary>
		/// <param name="item">The element to remove from the dictionary</param>
		/// <returns>true if the element was successfully removed from the dictionary, false otherwise</returns>
		public bool Remove(KeyValuePair<TKey,TValue> item)
		{
            bool removed = false;

            if(m_Data.TryGetValue(item.Key, out var values))
            {
                removed = values.Remove(item.Value);
            }

            return removed;
        }
		
		/// <summary>
		/// Returns an enumerator containing a key and all values in it sequence
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerable<KeyValuePair<TKey,IList<TValue>>> Sequences
		{
			get
			{
				foreach(KeyValuePair<TKey,IList<TValue>> pair in m_Data)
				{
					yield return pair;
				}
			}
		}

        /// <summary>
        /// Returns an enumerator that iterates throught the collection
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection</returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            foreach(KeyValuePair<TKey, IList<TValue>> pair in m_Data)
            {
                TKey key = pair.Key;
                foreach(TValue value in pair.Value)
                {
                    yield return new KeyValuePair<TKey, TValue>(key, value);
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates throught the collection
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        private IList<TValue> GetOrCreate(TKey key)
        {
            if(m_Data.TryGetValue(key, out var values) == false)
            {
                values = new List<TValue>();
                m_Data.Add(key, values);
            }

            return values;
        }
    }
}
