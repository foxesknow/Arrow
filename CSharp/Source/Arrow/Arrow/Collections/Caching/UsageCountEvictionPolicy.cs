using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Arrow.Execution;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// Evicts an item after a specific number of uses
	/// </summary>
	public class UsageCountEvictionPolicy : EvictionPolicy
	{
		private readonly long m_NumberOfUses;
		private long m_UsesSoFar;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="numberOfUses">How many uses to allow</param>
		public UsageCountEvictionPolicy(long numberOfUses)
		{
			if(numberOfUses<0) throw new ArgumentException("numberOfUses");
			m_NumberOfUses=numberOfUses;
			m_UsesSoFar=0;
		}

		/// <summary>
		/// Determines if it is time to evict the item
		/// </summary>
		/// <returns>true if the item has been used the specified number of times, otherwise false</returns>
		public override bool MustEvict()
		{
			return Interlocked.Read(ref m_UsesSoFar)>=m_NumberOfUses;
		}

		/// <summary>
		/// Records the usage of the cached item
		/// </summary>
		public override void ItemTouched()
		{
			Interlocked.Increment(ref m_UsesSoFar);
		}

		/// <summary>
		/// Renders as a string
		/// </summary>
		/// <returns>A string</returns>
		public override string ToString()
		{
			return string.Format("Allows uses={0}, so far={1}",m_NumberOfUses,Interlocked.Read(ref m_UsesSoFar));
		}
	}

	public static partial class EvictionPolicyExtensions
	{
		/// <summary>
		/// Allows the cached item to be used a specific number of times
		/// </summary>
		/// <param name="factory">The factory we're extending</param>
		/// <param name="numberOfUses">How many uses to allow</param>
		/// <returns>An eviction policy</returns>
		public static EvictionPolicy NumberOfUses(this ExtensionPoint<EvictionPolicy> factory, int numberOfUses)
		{
			return new UsageCountEvictionPolicy(numberOfUses);
		}
	}
}
