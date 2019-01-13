using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Threading;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading
{
	[TestFixture]
	public class OutstandingEventTests
	{
		[Test]
		public void Creation()
		{
			using(OutstandingEvent e=new OutstandingEvent())
			{
				Assert.That(e.NothingOutstandingHandle.WaitOne(0,false),Is.True);
				Assert.That(e.SomethingOutstandingHandle.WaitOne(0,false),Is.False);
			}
		}
		
		[Test]
		public void IncrementByOne()
		{
			using(OutstandingEvent e=new OutstandingEvent())
			{
				e.Increase();
				
				Assert.That(e.NothingOutstandingHandle.WaitOne(0,false),Is.False);
				Assert.That(e.SomethingOutstandingHandle.WaitOne(0,false),Is.True);
				
				e.Decrease();
			
				// The decrease should have returned the handles to their original state
				Assert.That(e.NothingOutstandingHandle.WaitOne(0,false),Is.True);
				Assert.That(e.SomethingOutstandingHandle.WaitOne(0,false),Is.False);
			}
		}
		
		[Test]
		public void IncrementSeveral()
		{
			using(OutstandingEvent e=new OutstandingEvent())
			{
				e.Increase(10);				
				Assert.That(e.NothingOutstandingHandle.WaitOne(0,false),Is.False);
				Assert.That(e.SomethingOutstandingHandle.WaitOne(0,false),Is.True);
				
				// The handles state should be the same
				e.Decrease();
				Assert.That(e.NothingOutstandingHandle.WaitOne(0,false),Is.False);
				Assert.That(e.SomethingOutstandingHandle.WaitOne(0,false),Is.True);
			
				// The decrease should have returned the handles to their original state
				e.Decrease(9);
				Assert.That(e.NothingOutstandingHandle.WaitOne(0,false),Is.True);
				Assert.That(e.SomethingOutstandingHandle.WaitOne(0,false),Is.False);
			}
		}
		
		[Test]
		public void DecreaseToNegative()
		{
            Assert.Throws<ArgumentException>(() =>
            {
			    using(OutstandingEvent e=new OutstandingEvent())
			    {
				    e.Decrease(1);
				    Assert.Fail(); // Shouldn't get here
			    }
            });
		}
	}
}
