using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
	[TestFixture]
	public class IgnoreCaseComparerTests
	{
		[Test]
		public void Test()
		{
			var c=new IgnoreCaseComparer();
			
			Assert.That(c.Compare("hello","hello"),Is.EqualTo(0));
			Assert.That(c.Compare("hello","HELLO"),Is.EqualTo(0));
			Assert.That(c.Compare("HELLO","Hello"),Is.EqualTo(0));			
			Assert.That(c.Compare("HELLO","WORLD"),Is.Not.EqualTo(0));			
		}
	}
}
