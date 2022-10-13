using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections.Caching;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections.Caching
{
	[TestFixture]
	public class FactoryTests
	{
		[Test]
		public void Lookup()
		{
			var cache=CreateCache();

			int value;
			bool found=cache.Lookup(10,out value);
			Assert.That(found,Is.True);
			Assert.That(value,Is.EqualTo(100));
		}

		[Test]
		public void LookupAfterClear()
		{
			var cache=CreateCache();

			int value;
			bool found=cache.Lookup(10,out value);
			Assert.That(found,Is.True);
			Assert.That(value,Is.EqualTo(100));

			cache.Clear();
			Assert.That(cache.ItemsCached,Is.EqualTo(0));

			found=cache.Lookup(10,out value);
			Assert.That(found,Is.True);
			Assert.That(value,Is.EqualTo(100));
		}

		private ICache<int,int> CreateCache()
		{
			return new Cache<int,int>(Factory);
		}

		private bool Factory(int key, out int value, out EvictionPolicy policy)
		{
			value=key*key;
			policy=EvictionPolicy.Factory.LiveForever();
			return true;
		}
	}
}
