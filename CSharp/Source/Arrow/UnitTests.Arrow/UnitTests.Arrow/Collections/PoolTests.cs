using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Collections
{
	[TestFixture]
	public class PoolTests
	{
		[Test]
		public void Checkout()
		{
			var pool=CreatePool();
			
			int value0=pool.Checkout();
			Assert.That(value0,Is.EqualTo(0));

			int value1=pool.Checkout();
			Assert.That(value1,Is.EqualTo(1));
		}

		[Test]
		public void TryCheckout()
		{
			var pool=CreatePool();

			int temp=0;
			Assert.That(pool.TryCheckout(out temp),Is.False);
			
			int value0=pool.Checkout();
			int value1=pool.Checkout();
			
			pool.Checkin(value0);			
			pool.Checkin(value1);

			Assert.That(pool.TryCheckout(out temp),Is.True);
			Assert.That(temp,Is.EqualTo(1));
		}

		[Test]
		public void Checkin()
		{
			var pool=CreatePool();
			
			int value0=pool.Checkout();
			int value1=pool.Checkout();

			pool.Checkin(value0);
			pool.Checkin(value1);

			// value1 (which is 1) should be the next available value
			int value=pool.Checkout();
			Assert.That(value,Is.EqualTo(1));
		}

		[Test]
		public void Count()
		{
			var pool=CreatePool();
			Assert.That(pool.Count,Is.EqualTo(0));
			
			int value0=pool.Checkout();
			Assert.That(pool.Count,Is.EqualTo(0));

			int value1=pool.Checkout();
			Assert.That(pool.Count,Is.EqualTo(0));

			pool.Checkin(value0);
			Assert.That(pool.Count,Is.EqualTo(1));

			pool.Checkin(value1);
			Assert.That(pool.Count,Is.EqualTo(2));
		}


		private Pool<int> CreatePool()
		{
			int counter=0;
			Func<int> factory=()=>counter++;

			var pool=new Pool<int>(factory);
			return pool;
		}
	}
}
