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
	public class AlwaysAllocatePoolTests
	{
		[Test]
		public void Checkout()
		{
			var pool=new AlwaysAllocatePool();

			for(int i=0; i<10; i++)
			{
				int size=(i+1)*819;
				var buffer=pool.Checkout(size);

				Assert.That(buffer,Is.Not.Null);
				Assert.That(buffer.Length,Is.EqualTo(size));

				pool.Checkin(buffer);
			}
		}
	}
}
