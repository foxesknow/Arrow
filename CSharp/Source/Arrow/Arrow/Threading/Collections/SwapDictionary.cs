using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Arrow.Threading.Collections
{
	/// <summary>
	/// This class provides compare and swap (CAS) versions of all the mutatable methods
	/// in the generic Dictionary class. It does this my creating a copy of the source dictionary,
	/// applying the mutatable operation and then swapping the new list back in.
	/// 
	/// Because of the copy and swap semantics of this class it is best used in situations
	/// where the source dictionary is not particulary large or where the dictionary will be
	/// written to occassionaly but read many more times.
	/// </summary>
	public class SwapDictionary<K,V> : IDictionary<K,V>
	{
		private Dictionary<K,V> m_Dictionary;		

		/// <summary>
		/// Initializes the instance
		/// </summary>
		public SwapDictionary()
		{
			m_Dictionary=new Dictionary<K,V>();
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="dictionary">A dictionary whose values will to populate the instance</param>
		public SwapDictionary(IDictionary<K,V> dictionary)
		{
			m_Dictionary=new Dictionary<K,V>(dictionary);
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="comparer">The key comparer to use</param>
		public SwapDictionary(IEqualityComparer<K> comparer)
		{
			m_Dictionary=new Dictionary<K,V>(comparer);
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="capacity">The initial capacity of the dictionary</param>
		public SwapDictionary(int capacity)
		{
			m_Dictionary=new Dictionary<K,V>(capacity);
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="capacity">The initial capacity of the dictionay</param>
		/// <param name="comparer">The key comparer to use</param>
		public SwapDictionary(int capacity, IEqualityComparer<K> comparer)
		{
			m_Dictionary=new Dictionary<K,V>(capacity,comparer);
		}

		/// <summary>
		/// Returns the key comparer used by the dictionary
		/// </summary>
		public IEqualityComparer<K> Comparer
		{
			get
			{
				var dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
				return dictionary.Comparer;
			}
		}

		/// <summary>
		/// Adds to the dictionary
		/// </summary>
		/// <param name="key">The key to add</param>
		/// <param name="value">The value for the key</param>
		public void Add(K key, V value)
		{
			Dictionary<K,V> current;
			Dictionary<K,V> existing=m_Dictionary;

			do
			{
				current=existing;
				var newDictionary=Copy(current);
				newDictionary.Add(key,value);

				existing=Interlocked.CompareExchange(ref m_Dictionary,newDictionary,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Indicates if a key exists
		/// </summary>
		/// <param name="key">The key to check for</param>
		/// <returns>true if the key exists, otherwise false</returns>
		public bool ContainsKey(K key)
		{
			var dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
			return dictionary.ContainsKey(key);
		}

		/// <summary>
		/// Returns the keys in the dictionary
		/// </summary>
		public ICollection<K> Keys
		{
			get
			{
				var dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
				return dictionary.Keys;
			}
		}

		/// <summary>
		/// Returns the values in the dictionary
		/// </summary>
		public ICollection<V> Values
		{
			get
			{
				var dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
				return dictionary.Values;
			}
		}

		/// <summary>
		/// Attempts to remove a key from the dictionary
		/// </summary>
		/// <param name="key">The key to remove</param>
		/// <returns>true if the key was removed, otherwise false</returns>
		public bool Remove(K key)
		{
			bool removed=false;

			Dictionary<K,V> current;
			Dictionary<K,V> existing=m_Dictionary;

			do
			{
				current=existing;
				var newDictionary=Copy(current);
				removed=newDictionary.Remove(key);

				existing=Interlocked.CompareExchange(ref m_Dictionary,newDictionary,current);
			}while(current!=existing);

			return removed;
		}

		/// <summary>
		/// Attempts to remove a key from the dictionary, 
		/// returning the value that was mapped to the key
		/// if found
		/// </summary>
		/// <param name="key">The key to remove</param>
		/// <param name="value">The value of the key, if present</param>
		/// <returns>true if the key was found and removed, false otherwise</returns>
		public bool Remove(K key, out V value)
		{
			bool removed=false;

			Dictionary<K,V> current;
			Dictionary<K,V> existing=m_Dictionary;

			do
			{
				current=existing;

				bool found=current.TryGetValue(key,out value);
				if(found==false) return false;

				var newDictionary=Copy(current);
				removed=newDictionary.Remove(key);

				existing=Interlocked.CompareExchange(ref m_Dictionary,newDictionary,current);
			}while(current!=existing);

			return removed;
		}

		/// <summary>
		/// Attempts to get a value for a key
		/// </summary>
		/// <param name="key">The key to lookup</param>
		/// <param name="value">On success the value for the key, on failure the default for V</param>
		/// <returns>true if the key was found, otherwise false</returns>
		public bool TryGetValue(K key, out V value)
		{
			var dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
			return dictionary.TryGetValue(key,out value);
		}

		/// <summary>
		/// Gets a value for a key, or if the key isn't present
		/// create a value and adds it.
		/// </summary>
		/// <param name="key">The key to lookup</param>
		/// <param name="creator">A factory that will be call to create the value, if the key does not exist</param>
		/// <returns>The value for the key</returns>
		public V GetValueOrAdd(K key, Func<K,V> creator)
		{
			bool createdValue=false;
			V value=default(V);

			Dictionary<K,V> current;
			Dictionary<K,V> existing=m_Dictionary;

			do
			{
				current=existing;

				V existingValue;
				bool found=current.TryGetValue(key,out existingValue);
				if(found) return existingValue;

				// Avoid creating the value more than once
				if(!createdValue)
				{
					value=creator(key);
					createdValue=true;
				}

				var newDictionary=Copy(current);
				newDictionary.Add(key,value);

				existing=Interlocked.CompareExchange(ref m_Dictionary,newDictionary,current);
			}while(current!=existing);

			return value;
		}

		/// <summary>
		/// Gets or sets the value for a specified key
		/// </summary>
		/// <param name="key">The key to get or set</param>
		/// <returns>The value for the key</returns>
		public V this[K key]
		{
			get
			{
				var dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
				return dictionary[key];
			}
			set
			{
				Dictionary<K,V> current;
				Dictionary<K,V> existing=m_Dictionary;

				do
				{
					current=existing;
					var newDictionary=Copy(current);
					newDictionary[key]=value;

					existing=Interlocked.CompareExchange(ref m_Dictionary,newDictionary,current);
				}while(current!=existing);
			}
		}		

		/// <summary>
		/// Clears the contents of the dictionary
		/// </summary>
		public void Clear()
		{
			Dictionary<K,V> current;
			Dictionary<K,V> existing=m_Dictionary;

			do
			{
				current=existing;
				var newDictionary=Copy(current);
				newDictionary.Clear();

				existing=Interlocked.CompareExchange(ref m_Dictionary,newDictionary,current);
			}while(current!=existing);
		}		

		/// <summary>
		/// Copies the entire list to a compatible 1D array, starting at the specified index in the target array
		/// </summary>
		/// <param name="array">The array to write to</param>
		/// <param name="arrayIndex">The index at which copying should begin</param>
		public void CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			ICollection<KeyValuePair<K,V>> dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
			dictionary.CopyTo(array,arrayIndex);
		}

		/// <summary>
		/// Returns the number of items in the dictionary
		/// </summary>
		public int Count
		{
			get
			{
				var dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
				return dictionary.Count;
			}
		}

		/// <summary>
		/// Returns false
		/// </summary>
		public bool IsReadOnly
		{
			get{return false;}
		}		

		/// <summary>
		/// Returns an enumerator to the items in the dictionary
		/// </summary>
		/// <returns>An enumerator</returns>
		public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
		{
			var dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
			return dictionary.GetEnumerator();
		}
		
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds a pair to the dictionary
		/// </summary>
		/// <param name="item">The pair to add</param>
		void ICollection<KeyValuePair<K,V>>.Add(KeyValuePair<K, V> item)
		{
			Dictionary<K,V> current;
			Dictionary<K,V> existing=m_Dictionary;

			do
			{
				current=existing;
				var newDictionary=Copy(current);
				((ICollection<KeyValuePair<K,V>>)newDictionary).Add(item);

				existing=Interlocked.CompareExchange(ref m_Dictionary,newDictionary,current);
			}while(current!=existing);
		}

		/// <summary>
		/// Determines if a pair exists
		/// </summary>
		/// <param name="item">The pair to check for</param>
		/// <returns>true if the pair exists, otherwise false</returns>
		bool ICollection<KeyValuePair<K,V>>.Contains(KeyValuePair<K, V> item)
		{
			ICollection<KeyValuePair<K,V>> dictionary=Interlocked.CompareExchange(ref m_Dictionary,null,null);
			return dictionary.Contains(item);
		}

		/// <summary>
		/// Removes a pair from the dictionary
		/// </summary>
		/// <param name="item">The pair to remove</param>
		/// <returns>true if the pair was removed, otherwise false</returns>
		bool ICollection<KeyValuePair<K,V>>.Remove(KeyValuePair<K, V> item)
		{
			bool removed=false;

			Dictionary<K,V> current;
			Dictionary<K,V> existing=m_Dictionary;

			do
			{
				current=existing;
				var newDictionary=Copy(current);
				removed=((ICollection<KeyValuePair<K,V>>)newDictionary).Remove(item);

				existing=Interlocked.CompareExchange(ref m_Dictionary,newDictionary,current);
			}while(current!=existing);

			return removed;
		}

		private Dictionary<K,V> Copy(Dictionary<K,V> dictionary)
		{
			Dictionary<K,V> newDictionary=new Dictionary<K,V>(dictionary,dictionary.Comparer);
			return newDictionary;
		}
	}
}
