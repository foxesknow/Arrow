using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
	[TestFixture]
	public class IgnoreCaseEqualityComparerTests
	{
		[Test]
		public void Test()
		{
			var c=new IgnoreCaseEqualityComparer();
			
			Assert.IsTrue(c.Equals("hello","hello"));
			Assert.IsTrue(c.Equals("hello","HELLO"));
			Assert.IsTrue(c.Equals("HELLO","Hello"));
			
			Assert.IsFalse(c.Equals("HELLO","WORLD"));
			
			Assert.IsTrue(c.GetHashCode("hello")==c.GetHashCode("hello"));
			Assert.IsTrue(c.GetHashCode("hello")==c.GetHashCode("HELLO"));
		}
	}
}
