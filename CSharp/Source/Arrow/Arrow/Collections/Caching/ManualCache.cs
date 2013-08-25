using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// A manual cache requires the user to explicitly add items to the cache
	/// </summary>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="V"></typeparam>
	public class ManualCache<K,V> : CacheImpl<K,V>, ICache<K,V>
	{
		/// <summary>
		/// Initializes the instance. 
		/// The purge mode will be set to managed
		/// </summary>
		public ManualCache() : base(null,PurgeMode.Managed)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="purgeMode">The purge mode to use</param>
		public ManualCache(PurgeMode purgeMode) : base(null,purgeMode)
		{
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="comparer">The equality comparer to use for the key. May be null</param>
		/// <param name="purgeMode">The purge mode to use</param>
		public ManualCache(IEqualityComparer<K> comparer, PurgeMode purgeMode) : base(comparer,purgeMode)
		{
		}		

		/// <summary>
		/// Looks for a value in the cache
		/// NOTE: This method is a purge point
		/// NOTE: This method caused the ItemTouched() method on the policy to be called
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">On success the value, otherwise the default value for the type</param>
		/// <returns>true if the item was found, otherwise false</returns>
		public bool Lookup(K key, out V value)
		{
			return base.DoLookup(key,out value);
		}
	}
}
