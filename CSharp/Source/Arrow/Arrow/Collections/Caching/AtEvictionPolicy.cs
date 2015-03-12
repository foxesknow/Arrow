using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Calendar;
using Arrow.Execution;

namespace Arrow.Collections.Caching
{
	/// <summary>
	/// Evicts an item at a specific point in time
	/// </summary>
	public class AtEvictionPolicy : EvictionPolicy
	{
		private readonly DateTime m_WhenUtc;

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="when">When to evict the item</param>
		public AtEvictionPolicy(DateTime when)
		{
			m_WhenUtc=when.ToUniversalTime();
		}

		/// <summary>
		/// Determines if it is time to evict the item
		/// </summary>
		/// <returns>true if the specified time has been reached, otherwise false</returns>
		public override bool MustEvict()
		{
			var now=Clock.UtcNow;
			bool evict=(now>=m_WhenUtc);

			return evict;
		}

		/// <summary>
		/// Does nothing
		/// </summary>
		public override void ItemTouched()
		{
			// Does nothing
		}

		/// <summary>
		/// Renders the policy
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Evicting at {0}",m_WhenUtc);
		}
	}

	public static partial class EvictionPolicyExtensions
	{
		/// <summary>
		/// Evicts an item at a specific point in time
		/// </summary>
		/// <param name="factory">The factory we're extending</param>
		/// <param name="when">When the item should be evicted</param>
		/// <returns>An eviction policy</returns>
		public static EvictionPolicy At(this ExtensionPoint<EvictionPolicy> factory, DateTime when)
		{
			return new AtEvictionPolicy(when);
		}

		/// <summary>
		/// Evicts an item after a specific amount of time
		/// </summary>
		/// <param name="factory">The factory we're extending</param>
		/// <param name="duration">How long until we evict the item</param>
		/// <returns>An eviction policy</returns>
		public static EvictionPolicy In(this ExtensionPoint<EvictionPolicy> factory, TimeSpan duration)
		{
			var now=Clock.UtcNow;
			var when=now+duration;

			return new AtEvictionPolicy(when);
		}
	}
}
