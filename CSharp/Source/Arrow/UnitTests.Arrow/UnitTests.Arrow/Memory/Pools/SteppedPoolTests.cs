using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Memory.Pools;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Memory.Pools
{
	[TestFixture]
	public class SteppedPoolTests
	{
		[Test]
		public void Construct_BadArguments()
		{
			Assert.Throws<ArgumentException>(()=>
			{
				// An invalid number of steps (we need at least 1)
				var pool=new SteppedPool(0);
			});

			Assert.Throws<ArgumentException>(()=>
			{
				// An invalid step size (we need at least 1K per step)
				var pool=new SteppedPool(10,0);
			});
		}

		[Test]
		public void Construct_PreAllocate()
		{
			var pool=new SteppedPool(5,1,PoolMode.PreAllocate);
			Assert.That(pool.NumberOfSteps,Is.EqualTo(pool.GetAvailableSteps()));
		}

		[Test]
		public void Construct_NoPreAllocate()
		{
			var pool=new SteppedPool(5,1);
			Assert.That(pool.GetAvailableSteps(),Is.EqualTo(0));
		}

		[Test]
		public void CheckStepSizes_1K()
		{
			var pool=new SteppedPool(5,1);

			for(int i=0; i<pool.NumberOfSteps; i++)
			{
				int toAllocate=(i+1)*1024;
				
				var buffer=pool.Checkout(toAllocate);
				Assert.That(buffer,Is.Not.Null);
				Assert.That(buffer.Length,Is.EqualTo(toAllocate));	
			}

			// We didn't checkin, so nothing should be available
			Assert.That(pool.GetAvailableSteps(),Is.EqualTo(0));
		}

		[Test]
		public void CheckStepSizes_1K_Minus2()
		{
			var pool=new SteppedPool(5,1);

			for(int i=0; i<pool.NumberOfSteps; i++)
			{
				int roundedSize=(i+1)*1024;
				
				var buffer=pool.Checkout(roundedSize-2);
				Assert.That(buffer,Is.Not.Null);
				Assert.That(buffer.Length,Is.EqualTo(roundedSize));	

				pool.Checkin(buffer);
			}

			// We checked back in, so all the steps should be available
			Assert.That(pool.GetAvailableSteps(),Is.EqualTo(5));
		}

		[Test]
		public void CheckStepSizes_3K()
		{
			var pool=new SteppedPool(5,3);

			for(int i=0; i<pool.NumberOfSteps; i++)
			{
				int toAllocate=(i+1)*1024*3;
				
				var buffer=pool.Checkout(toAllocate);
				Assert.That(buffer,Is.Not.Null);
				Assert.That(buffer.Length,Is.EqualTo(toAllocate));	
			}

			// We didn't checkin, so nothing should be available
			Assert.That(pool.GetAvailableSteps(),Is.EqualTo(0));
		}

		[Test]
		public void CheckStepSizes_3K_Minus2()
		{
			var pool=new SteppedPool(5,3);

			for(int i=0; i<pool.NumberOfSteps; i++)
			{
				int roundedSize=(i+1)*1024*3;
				
				var buffer=pool.Checkout(roundedSize-2);
				Assert.That(buffer,Is.Not.Null);
				Assert.That(buffer.Length,Is.EqualTo(roundedSize));	

				pool.Checkin(buffer);
			}

			// We checked back in, so all the steps should be available
			Assert.That(pool.GetAvailableSteps(),Is.EqualTo(5));
		}

		[Test]
		public void RequestBufferLargeThanPool()
		{
			var pool=new SteppedPool(5,3,PoolMode.PreAllocate);

			Assert.That(pool.GetAvailableSteps(),Is.EqualTo(5));

			// The largest buffer in the pool is 15K, so this is way too big
			var buffer=pool.Checkout(64000);
			Assert.That(buffer,Is.Not.Null);

			Assert.That(pool.GetAvailableSteps(),Is.EqualTo(5));
			pool.Checkin(buffer);
			Assert.That(pool.GetAvailableSteps(),Is.EqualTo(5));
		}

		[Test]
		public void Buffer_NotClear()
		{
			var pool=new SteppedPool(5,3);

			int size=56;
			var buffer=pool.Checkout(size);
			
			// Initially each item is default(byte);
			for(int i=0; i<size; i++)
			{
				Assert.That(buffer[i],Is.EqualTo(0));
				buffer[i]=99;
			}

			pool.Checkin(buffer);

			buffer=pool.Checkout(size);
			
			// We should see the old data there as the pool isn't configured to clear it
			for(int i=0; i<size; i++)
			{
				Assert.That(buffer[i],Is.EqualTo(99));
			}
		}

		[Test]
		public void Buffer_ClearedOnCheckout()
		{
			var pool=new SteppedPool(5,3,PoolMode.ClearOnCheckout);

			int size=56;
			var buffer=pool.Checkout(size);
			
			// Initially each item is default(byte);
			for(int i=0; i<size; i++)
			{
				Assert.That(buffer[i],Is.EqualTo(0));
				buffer[i]=99;
			}

			pool.Checkin(buffer);

			buffer=pool.Checkout(size);
			
			// We should see the old data there as the pool isn't configured to clear it
			for(int i=0; i<size; i++)
			{
				Assert.That(buffer[i],Is.EqualTo(0));
			}
		}
	}
}
