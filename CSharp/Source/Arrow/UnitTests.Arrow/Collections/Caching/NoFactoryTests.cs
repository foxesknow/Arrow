using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections.Caching;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections.Caching
{
	[TestFixture]
	public class NoFactoryTests
	{
		[Test]
		public void Add()
		{
			var cache=CreateCache();

			cache.Add(10,"100",EvictionPolicy.Factory.LiveForever());

			string value;
			bool found=cache.Lookup(10,out value);

			Assert.That(found,Is.True);
			Assert.That(value,Is.EqualTo("100"));
		}

		[Test]
		public void Lookup()
		{
			var cache=CreateCache();

			string value;
			bool found=cache.Lookup(1,out value);
			Assert.That(found,Is.False);
			Assert.That(value,Is.Null);

			cache.Add(1,"Jack",EvictionPolicy.Factory.LiveForever());
			found=cache.Lookup(1,out value);
			Assert.That(found,Is.True);
			Assert.That(value,Is.EqualTo("Jack"));
		}

		[Test]
		public void Clear()
		{
			var cache=CreateCache();

			Assert.That(cache.ItemsCached,Is.EqualTo(0));
			cache.Clear();
			Assert.That(cache.ItemsCached,Is.EqualTo(0));

			cache.Add(10,"100",EvictionPolicy.Factory.LiveForever());
			Assert.That(cache.ItemsCached,Is.EqualTo(1));

			cache.Clear();
			Assert.That(cache.ItemsCached,Is.EqualTo(0));
		}

		[Test]
		public void ItemsCached()
		{
			var cache=CreateCache();

			Assert.That(cache.ItemsCached,Is.EqualTo(0));

			cache.Add(10,"100",EvictionPolicy.Factory.LiveForever());
			Assert.That(cache.ItemsCached,Is.EqualTo(1));

			cache.Add(2,"4",EvictionPolicy.Factory.LiveForever());
			Assert.That(cache.ItemsCached,Is.EqualTo(2));

			cache.Clear();
			Assert.That(cache.ItemsCached,Is.EqualTo(0));
		}

		[Test]
		public void Keys()
		{
			var cache=CreateCache();

			var keys=cache.Keys();
			Assert.That(keys.Count(),Is.EqualTo(0));

			cache.Add(10,"100",EvictionPolicy.Factory.LiveForever());
			keys=cache.Keys();
			Assert.That(keys.Count(),Is.EqualTo(1));
			Assert.That(keys.Contains(10),Is.True);
		}

		[Test]
		public void ItemEvicted()
		{
			var cache=CreateCache();
			cache.Add(10,"Jack",EvictionPolicy.Factory.LiveForever());
			cache.Add(20,"Sawyer",EvictionPolicy.Factory.LiveForever());

			int itemsEvicted=0;
			cache.ItemEvicted+=(s,e)=>itemsEvicted++;

			cache.Clear();
			Assert.That(itemsEvicted,Is.EqualTo(2));
		}

		private ICache<int,string> CreateCache()
		{
			return new Cache<int,string>();
		}
	}
}
