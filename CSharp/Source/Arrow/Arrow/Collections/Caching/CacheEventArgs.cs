using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// Event class for cache events
	/// </summary>
	/// <typeparam name="K"></typeparam>
	/// <typeparam name="V"></typeparam>
	public class CacheEventArgs<K,V> : EventArgs
	{
		private readonly K m_Key;
		private readonly V m_Value;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public CacheEventArgs(K key, V value)
		{
			m_Key=key;
			m_Value=value;
		}

		/// <summary>
		/// The cache key
		/// </summary>
		public K Key
		{
			get{return m_Key;}
		}

		/// <summary>
		/// The cache value
		/// </summary>
		public V Value
		{
			get{return m_Value;}
		}
	}
}
