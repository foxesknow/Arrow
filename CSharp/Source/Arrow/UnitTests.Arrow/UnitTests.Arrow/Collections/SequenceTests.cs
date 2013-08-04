using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
	[TestFixture]
	public class SequenceTests
	{
		[Test]
		public void Cons()
		{
			int[] numbers={1,2,3};

			var enumerator=Sequence.Cons(0,numbers).GetEnumerator();
			
			enumerator.MoveNext();
			Assert.That(enumerator.Current,Is.EqualTo(0));

			enumerator.MoveNext();
			Assert.That(enumerator.Current,Is.EqualTo(1));

			enumerator.MoveNext();
			Assert.That(enumerator.Current,Is.EqualTo(2));

			enumerator.MoveNext();
			Assert.That(enumerator.Current,Is.EqualTo(3));
		}

		[Test]
		public void ToEnumerable()
		{
			var enumerator=Sequence.Single(1).GetEnumerator();

			enumerator.MoveNext();
			Assert.That(enumerator.Current,Is.EqualTo(1));
		}
	}
}
