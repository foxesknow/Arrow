using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections.Caching
{	
	/// <summary>
	/// Defines a reasonable implementation of the ICache interface
	/// </summary>
	/// <typeparam name="K">The type of the cache key</typeparam>
	/// <typeparam name="V">The type of the cache value</typeparam>
	public abstract class CacheImpl<K,V>
	{
		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<K,CacheItem<V>> m_Cache;
		private readonly PurgeMode m_PurgeMode;

		/// <summary>
		/// Raised when an item is evicted from the cache
		/// </summary>
		public event EventHandler<CacheEventArgs<K,V>> ItemEvicted;

		protected CacheImpl() : this(null,PurgeMode.Managed)
		{
		}

		protected CacheImpl(PurgeMode purgeMode) : this(null,purgeMode)
		{
		}

		protected CacheImpl(IEqualityComparer<K> comparer, PurgeMode purgeMode)
		{
			m_Cache=new Dictionary<K,CacheItem<V>>(comparer);
			m_PurgeMode=purgeMode;
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
				foreach(var pair in m_Cache)
				{
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
		/// The cached items.
		/// Access to this should be controlled via a SyncRoot lock
		/// </summary>
		protected IDictionary<K,CacheItem<V>> Cache
		{
			get{return m_Cache;}
		}

		/// <summary>
		/// Looks for an item in the cache
		/// </summary>
		/// <param name="key">The key to look up</param>
		/// <param name="value">The value for the key if successful</param>
		/// <returns>true if the item was found, otherwise false</returns>
		protected bool DoLookup(K key, out V value)
		{
			if(key==null) throw new ArgumentNullException("key");

			lock(this.SyncRoot)
			{
				CheckAndPurge();

				bool found=false;

				CacheItem<V> cacheItem;
				if(this.Cache.TryGetValue(key,out cacheItem))
				{
					value=cacheItem.Item;
					cacheItem.Policy.ItemTouched();
					found=true;

				}
				else
				{
					value=default(V);
					found=false;
				}

				return found;
			}
		}

		/// <summary>
		/// Checks for any purged items in the purge mode is managed
		/// </summary>
		protected void CheckAndPurge()
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
				Evict(pair.Key,pair.Value.Item);
			}
		}

		private void Evict(K key, V value)
		{
			if(m_Cache.Remove(key))
			{
				var args=new CacheEventArgs<K,V>(key,value);
				OnItemEvicted(args);
			}
		}

		protected virtual void OnItemEvicted(CacheEventArgs<K,V> args)
		{
			var d=ItemEvicted;
			if(d!=null) d(this,args);
		}
	}
}
