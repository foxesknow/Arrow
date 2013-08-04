using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Threading;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading
{
	[TestFixture]
	public class InterlockedExTests
	{
		[Test]
		public void IntOr()
		{
			int value=16;
			int previous=InterlockedEx.Transform(ref value,x=>x|2);
			
			Assert.That(previous,Is.EqualTo(16));
			Assert.That(value,Is.EqualTo(18));
		}
		
		[Test]
		public void LongOr()
		{
			long value=32;
			long previous=InterlockedEx.Transform(ref value,x=>x|2);
			
			Assert.That(previous,Is.EqualTo(32));
			Assert.That(value,Is.EqualTo(34));
		}
		
		[Test]
		public void ReferenceSet()
		{
			string name="James";
			string previous=InterlockedEx.Transform(ref name,x=>x+" Sawyer");
			
			Assert.That(previous,Is.EqualTo("James"));
			Assert.That(name,Is.EqualTo("James Sawyer"));
		}
	}
}
