using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections.Caching
{
	public class CacheEventArgs<K,V> : EventArgs
	{
		private readonly K m_Key;
		private readonly V m_Value;

		public CacheEventArgs(K key, V value)
		{
			m_Key=key;
			m_Value=value;
		}

		public K Key
		{
			get{return m_Key;}
		}

		public V Value
		{
			get{return m_Value;}
		}
	}
}
