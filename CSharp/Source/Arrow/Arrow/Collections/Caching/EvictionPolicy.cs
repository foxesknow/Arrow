using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Execution;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// Defines the eviction policy for individual items in the cache
	/// </summary>
	public abstract class EvictionPolicy
	{
		/// <summary>
		/// An extension point to allow policy factory methods to be bolted on
		/// </summary>
		public static readonly ExtensionPoint<EvictionPolicy> Factory=new ExtensionPoint<EvictionPolicy>();

		/// <summary>
		/// Indicates if the item the policy controls should be evicted
		/// </summary>
		/// <returns></returns>
		public abstract bool MustEvict();
		
		/// <summary>
		/// Called when an item has been touched by the cache
		/// </summary>
		public abstract void ItemTouched();
	}
}
