using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// A delegate that can create an item to be stored in a cache when required
	/// </summary>
	/// <typeparam name="K">The type of the key</typeparam>
	/// <typeparam name="V">The type of the value</typeparam>
	/// <param name="key">The key of the item that should be created</param>
	/// <param name="value">It successful the value for the key, otherwise the default value for the value</param>
	/// <param name="evictionPolicy">The eviction policy to use for the value</param>
	/// <returns>true if the item could be created, otherwise false</returns>
	public delegate bool CacheFactory<K,V>(K key, out V value, out EvictionPolicy evictionPolicy);
}
