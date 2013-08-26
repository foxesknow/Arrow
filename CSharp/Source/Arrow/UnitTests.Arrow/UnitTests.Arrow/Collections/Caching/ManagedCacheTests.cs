using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections.Caching;

using NUnit.Framework;


namespace UnitTests.Arrow.Collections.Caching
{
	[TestFixture]
	public class ManagedCacheTests
	{
		[Test]
		public void Lookup()
		{
			var cache=CreateCache();

			bool itemEvicted=false;
			cache.ItemEvicted+=(s,e)=>itemEvicted=true;

			cache.Add(10,"Jack",EvictionPolicy.Factory.NumberOfUses(1));

			string value;
			bool found=cache.Lookup(10,out value);
			Assert.That(found,Is.True);
			Assert.That(value,Is.EqualTo("Jack"));
			Assert.That(itemEvicted,Is.False);

			// The item should be gone now
			found=cache.Lookup(10,out value);
			Assert.That(found,Is.False);
			Assert.That(value,Is.Null);
			Assert.That(itemEvicted,Is.True);
		}

		private ICache<int,string> CreateCache()
		{
			return new Cache<int,string>(PurgeMode.Managed);
		}
	}
}
