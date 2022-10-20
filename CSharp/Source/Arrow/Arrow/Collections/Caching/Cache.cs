using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable

namespace Arrow.Collections.Caching
{	
	/// <summary>
	/// Defines a reasonable implementation of the ICache interface
	/// </summary>
	/// <typeparam name="K">The type of the cache key</typeparam>
	/// <typeparam name="V">The type of the cache value</typeparam>
	public class Cache<K,V> : ICache<K,V>
	{
		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<K,CacheItem<V>> m_Cache;
		private readonly PurgeMode m_PurgeMode;
		private readonly CacheFactory<K,V> m_Factory;

		/// <summary>
		/// Raised when an item is evicted from the cache
		/// </summary>
		public event EventHandler<CacheEventArgs<K,V>> ItemEvicted;

		/// <summary>
		/// Initializes the instance and sets the purge mode to managed
		/// </summary>
		public Cache() : this(null,PurgeMode.Managed,null)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="purgeMode">The purge mode to use</param>
		public Cache(PurgeMode purgeMode) : this(null,purgeMode,null)
		{
		}

		/// <summary>
		/// Initializes the instance and sets the purge mode to managed
		/// </summary>
		/// <param name="factory">The factory to call when Lookup cannot cannot find an item. May be null</param>
		public Cache(CacheFactory<K,V> factory) : this(null,PurgeMode.Managed,factory)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="purgeMode">The purge mode to use</param>
		/// <param name="factory">The factory to call when Lookup cannot cannot find an item. May be null</param>
		public Cache(PurgeMode purgeMode, CacheFactory<K,V> factory) : this(null,purgeMode,factory)
		{
		}

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="comparer"></param>
		/// <param name="purgeMode">The purge mode to use</param>
		/// <param name="factory">The factory to call when Lookup cannot cannot find an item. May be null</param>
		public Cache(IEqualityComparer<K> comparer, PurgeMode purgeMode, CacheFactory<K,V> factory)
		{
			m_Cache=new Dictionary<K,CacheItem<V>>(comparer);
			m_PurgeMode=purgeMode;
			m_Factory=factory;
		}

		/// <summary>
		/// Adds an item to the cache
		/// NOTE: This method is a purge point
		/// </summary>
		/// <param name="key">The key</param>
		/// <param name="value">The value</param>
		/// <param name="policy">The eviction policy</param>
		public void Add(K key, V value, EvictionPolicy policy)
		{
			if(key==null) throw new ArgumentNullException("key");
			if(policy==null) throw new ArgumentNullException("policy");

			var cacheItem=new CacheItem<V>(value,policy);

			lock(m_SyncRoot)
			{
				CheckAndPurge();
				m_Cache.Add(key,cacheItem);
			}
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
				CheckAndPurge();

				CacheItem<V> cacheItem;
				bool found=m_Cache.TryGetValue(key,out cacheItem);

				if(found)
				{
					cacheItem.Policy.ItemTouched();
					value=cacheItem.Item;
				}
				else
				{
					// If it's not being cached already then we need to try and create it
					if(m_Factory!=null)
					{
						V localValue;
						EvictionPolicy policy;
					
						if(m_Factory(key,out localValue,out policy))
						{
							if(policy==null) throw new ArgumentException("policy returned was null","policy");

							// We'll touch the item as we are returning it, 
							// and that will stay consistent with the behaviour of Lookup
							policy.ItemTouched();
						
							m_Cache.Add(key,new CacheItem<V>(localValue,policy));
							found=true;
							value=localValue;
						}
						else
						{
							found=false;
							value=default(V);
						}
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

		/// <summary>
		/// Purges any expired items from the cache
		/// </summary>
		public void Purge()
		{
			lock(m_SyncRoot)
			{
				PurgeCache();
			}
		}

		/// <summary>
		/// Returns all he keys in the cache
		/// NOTE: This method is a purge point
		/// </summary>
		/// <returns></returns>
		public IList<K> Keys()
		{
			lock(m_SyncRoot)
			{
				CheckAndPurge();

				List<K> keys=new List<K>(m_Cache.Count);
				keys.AddRange(m_Cache.Keys);

				return keys;
			}
		}

		/// <summary>
		/// Removes all items from the cache
		/// </summary>
		public void Clear()
		{
			lock(m_SyncRoot)
			{
				var items=m_Cache.ToList();

				foreach(var pair in items)
				{
					m_Cache.Remove(pair.Key);
					Evict(pair.Key,pair.Value.Item);
				}

				m_Cache.Clear();
			}
		}

		/// <summary>
		/// Returns the number of items held in the cache
		/// </summary>
		public int ItemsCached
		{
			get
			{
				lock(m_SyncRoot)
				{
					return m_Cache.Count;
				}
			}
		}
		
		/// <summary>
		/// The purge mode for the cache
		/// </summary>
		public PurgeMode PurgeMode
		{
			get{return m_PurgeMode;}
		}

		/// <summary>
		/// The object used to control access to the cache
		/// </summary>
		public object SyncRoot
		{
			get{return m_SyncRoot;}
		}

		/// <summary>
		/// Touches all items in the cache
		/// NOTE: This method is a purge point
		/// </summary>
		public void TouchAll()
		{
			lock(m_SyncRoot)
			{
				foreach(var item in m_Cache)
				{
					item.Value.Policy.ItemTouched();
				}

				CheckAndPurge();
			}
		}

		/// <summary>
		/// Evicts an item from the cache
		/// </summary>
		/// <param name="key">The key of the item to evict</param>
		/// <returns>true if the item was found and evicted, otherwise false</returns>
		public bool Evict(K key)
		{
			if(key==null) throw new ArgumentNullException("key");

			lock(m_SyncRoot)
			{
				CacheItem<V> item;
				bool found=m_Cache.TryGetValue(key,out item);
				
				if(found)
				{
					m_Cache.Remove(key);
					Evict(key,item.Item);
				}

				return found;
			}
		}

		/// <summary>
		/// Checks for any purged items in the purge mode is managed
		/// </summary>
		private void CheckAndPurge()
		{
			if(m_PurgeMode==PurgeMode.Managed)
			{
				lock(m_SyncRoot)
				{
					PurgeCache();
				}
			}
		}

		private void PurgeCache()
		{
			var itemsToEvict=new List<KeyValuePair<K,CacheItem<V>>>();

			foreach(var pair in m_Cache)
			{
				if(pair.Value.Policy.MustEvict())
				{
					itemsToEvict.Add(pair);
				}
			}

			foreach(var pair in itemsToEvict)
			{
				m_Cache.Remove(pair.Key);
				Evict(pair.Key,pair.Value.Item);
			}
		}

		private void Evict(K key, V value)
		{
			var args=new CacheEventArgs<K,V>(key,value);
			OnItemEvicted(args);
		}

		private void OnItemEvicted(CacheEventArgs<K,V> args)
		{
			var d=ItemEvicted;
			if(d!=null) d(this,args);
		}
	}
}
