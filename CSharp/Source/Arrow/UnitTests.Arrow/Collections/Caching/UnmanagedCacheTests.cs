using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections.Caching;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections.Caching
{
	[TestFixture]
	public class UnmanagedCacheTests
	{
		[Test]
		public void Lookup()
		{
			var cache=CreateCache();
			cache.Add(10,"Jack",EvictionPolicy.Factory.NumberOfUses(1));

			// We've only allowed one use, but because we're manual
			// purge it won't be evicted until we tell it to do
			for(int i=0; i<10; i++)
			{
				string value;
				bool found=cache.Lookup(10,out value);
				Assert.That(found,Is.True);
				Assert.That(value,Is.EqualTo("Jack"));
			}

			cache.Purge();
			{
				string value;
				bool found=cache.Lookup(10,out value);
				Assert.That(found,Is.False);
				Assert.That(value,Is.Null);
			}
		}

		private ICache<int,string> CreateCache()
		{
			return new Cache<int,string>(PurgeMode.Unmanaged);
		}
	}
}
