using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Execution;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// Defines an eviction policy that will never evict an item
	/// </summary>
	public class LiveForeverEvictionPolicy : EvictionPolicy
	{
		/// <summary>
		/// A singleton that can used to save memory
		/// </summary>
		public static readonly EvictionPolicy Instance=new LiveForeverEvictionPolicy();

		/// <summary>
		/// Always returns false
		/// </summary>
		/// <returns>false</returns>
		public override bool MustEvict()
		{
 			return false;
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		public override void ItemTouched()
		{
 			// Does nothing
		}
	}

	/// <summary>
	/// Useful eviction extensions
	/// </summary>
	public static partial class EvictionPolicyExtensions
	{
		/// <summary>
		/// Returns an eviction policy that will allow an item to 
		/// stay in the cache forever
		/// </summary>
		/// <param name="factory">The factory we're extending</param>
		/// <returns>An eviction policy</returns>
		public static EvictionPolicy LiveForever(this ExtensionPoint<EvictionPolicy> factory)
		{
			return LiveForeverEvictionPolicy.Instance;
		}
	}
}
