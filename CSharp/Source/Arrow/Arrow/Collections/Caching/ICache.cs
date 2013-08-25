using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// Defines the standard behaviour for a cache
	/// </summary>
	/// <typeparam name="K">The key of the item held in the cache</typeparam>
	/// <typeparam name="V">The value held in the cache</typeparam>
	public interface ICache<K,V>
	{
		/// <summary>
		/// Raised when data is evicted from the cache
		/// </summary>
		event EventHandler<CacheEventArgs<K,V>> ItemEvicted;

		/// <summary>
		/// Adds an item to the cache
		/// NOTE: This method is a purge point
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		/// <param name="policy">The eviction policy</param>
		void Add(K key, V value, EvictionPolicy policy);
		
		/// <summary>
		/// Looks for a value in the cache
		/// NOTE: This method is a purge point
		/// NOTE: This method caused the ItemTouched() method on the policy to be called
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">On success the value, otherwise the default value for the type</param>
		/// <returns>true if the item was found, otherwise false</returns>
		bool Lookup(K key, out V value);
		
		/// <summary>
		/// Purges any expired items from the cache
		/// </summary>
		void Purge();
		
		/// <summary>
		/// Returns all he keys in the cache
		/// NOTE: This method is a purge point
		/// </summary>
		/// <returns></returns>
		IList<K> Keys();
		
		/// <summary>
		/// Returns the number of items held in the cache
		/// </summary>
		int ItemsCached{get;}
		
		/// <summary>
		/// Removes all items from the cache
		/// </summary>
		void Clear();

		/// <summary>
		/// The object used to control access to the cache
		/// </summary>
		object SyncRoot{get;}

		/// <summary>
		/// The purge mode for the cache
		/// </summary>
		PurgeMode PurgeMode{get;}

		/// <summary>
		/// Touches all items in the cache
		/// NOTE: This method is a purge point
		/// </summary>
		void TouchAll();

	}
}
