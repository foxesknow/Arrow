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
	public class FixedSizePoolTests
	{
		[Test]
		public void Construct_BadArguments()
		{
			Assert.Throws<ArgumentException>(()=>
			{
				// We need at least one buffer to manage
				var pool=new FixedSizePool(0,50);
			});

			Assert.Throws<ArgumentException>(()=>
			{
				// The buffers need to be at least 1 byte in size
				var pool=new SteppedPool(10,0);
			});
		}

		[Test]
		public void NoBuffersAvailable()
		{
			var pool=new FixedSizePool(7,128);

			Assert.That(pool.AvailableBuffers,Is.EqualTo(0));
		}

		[Test]
		public void BufferPreAllocated()
		{
			var pool=new FixedSizePool(7,128,PoolMode.PreAllocate);

			Assert.That(pool.AvailableBuffers,Is.EqualTo(pool.NumberOfBuffers));
		}

		[Test]
		public void Checkout_No_BufferSize()
		{
			var pool=new FixedSizePool(7,128,PoolMode.PreAllocate);
			Assert.That(pool.AvailableBuffers,Is.EqualTo(pool.NumberOfBuffers));

			var buffer=pool.Checkout(1234);
			Assert.That(buffer,Is.Not.Null);

			// As we didn't ask for the buffer size all the buffers should still be there
			Assert.That(pool.AvailableBuffers,Is.EqualTo(pool.NumberOfBuffers));

			pool.Checkin(buffer);
			Assert.That(pool.AvailableBuffers,Is.EqualTo(pool.NumberOfBuffers));
		}

		[Test]
		public void Checkout()
		{
			var pool=new FixedSizePool(7,128,PoolMode.PreAllocate);
			Assert.That(pool.AvailableBuffers,Is.EqualTo(pool.NumberOfBuffers));

			var buffer=pool.Checkout(pool.BufferSize);
			Assert.That(buffer,Is.Not.Null);
			Assert.That(buffer.Length,Is.EqualTo(pool.BufferSize));

			// There should be one less buffer in the pool
			Assert.That(pool.AvailableBuffers,Is.EqualTo(pool.NumberOfBuffers-1));

			pool.Checkin(buffer);
			Assert.That(pool.AvailableBuffers,Is.EqualTo(pool.NumberOfBuffers));
		}

		[Test]
		public void Checkout_TooMany()
		{
			var pool=new FixedSizePool(7,128,PoolMode.PreAllocate);
			var buffers=new List<byte[]>();

			// First, grab way to many
			for(int i=0; i<100; i++)
			{
				var buffer=pool.Checkout(pool.BufferSize);
				Assert.That(buffer,Is.Not.Null);
				Assert.That(buffer.Length,Is.EqualTo(pool.BufferSize));

				buffers.Add(buffer);
			}

			Assert.That(pool.AvailableBuffers,Is.EqualTo(0));

			// Now put them back
			for(int i=0; i<100; i++)
			{
				var buffer=buffers[i];
				pool.Checkin(buffer);
			}

			Assert.That(pool.AvailableBuffers,Is.EqualTo(7));
		}
	}
}
