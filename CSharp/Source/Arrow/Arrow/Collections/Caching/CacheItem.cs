using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable disable

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// The items held in a cache
	/// </summary>
	/// <typeparam name="T">The type of the item held in the cache</typeparam>
	public sealed class CacheItem<T>
	{
		private readonly T m_Item;
		private readonly EvictionPolicy m_Policy;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="item">The item to hold</param>
		/// <param name="policy">The eviction policy for the item</param>
		public CacheItem(T item, EvictionPolicy policy)
		{
			m_Item=item;
			m_Policy=policy;
		}

		/// <summary>
		/// The item
		/// </summary>
		public T Item
		{
			get{return m_Item;}
		}

		/// <summary>
		/// The eviction policy
		/// </summary>
		public EvictionPolicy Policy
		{
			get{return m_Policy;}
		}

		/// <summary>
		/// Renders the item as a string
		/// </summary>
		/// <returns>A string</returns>
		public override string ToString()
		{
			return m_Item==null ? "null" : m_Item.ToString();
		}
	}
}
