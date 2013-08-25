using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections.Caching
{
	public class AutoCreateCache<K,V> : CacheImpl<K,V>, ICache<K,V>
	{
		private readonly CacheFactory<K,V> m_Factory;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="factory">A delegate to call when values need to be created from keys</param>
		public AutoCreateCache(CacheFactory<K,V> factory) : base(null,PurgeMode.Managed)
		{
			if(factory==null) throw new ArgumentNullException("factory");
			m_Factory=factory;
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="purgeMode">The purge mode to use</param>
		/// <param name="factory">A delegate to call when values need to be created from keys</param>
		public AutoCreateCache(PurgeMode purgeMode, CacheFactory<K,V> factory) : base(null,purgeMode)
		{
			if(factory==null) throw new ArgumentNullException("factory");
			m_Factory=factory;
		}
		
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="comparer">The equality comparer to use for the key. May be null</param>
		/// <param name="purgeMode">The purge mode to use</param>
		/// <param name="factory">A delegate to call when values need to be created from keys</param>
		public AutoCreateCache(IEqualityComparer<K> comparer, PurgeMode purgeMode, CacheFactory<K,V> factory) : base(comparer,purgeMode)
		{
			if(factory==null) throw new ArgumentNullException("factory");
			m_Factory=factory;
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
			lock(this.SyncRoot)
			{
				bool found=base.DoLookup(key,out value);

				// If it's not being cached already then we need to try and create it
				if(found==false)
				{
					V localValue;
					EvictionPolicy policy;
					
					if(m_Factory(key,out localValue,out policy))
					{
						if(policy==null) throw new ArgumentException("policy returned was null","policy");

						// We'll touch the item as we are returning it, 
						// and that will stay consistent with the behaviour of Lookup
						policy.ItemTouched();
						
						this.Cache.Add(key,new CacheItem<V>(localValue,policy));
						found=true;
						value=localValue;
					}
					else
					{
						found=false;
						value=default(V);
					}
				}

				return found;
			}
		}
	}
}
