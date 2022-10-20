using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void AddToEnd()
        {
            var numbers = new[]{1, 2, 3, 4};
            var sequence = numbers.AddToEnd(99);

            Assert.That(sequence.Last(), Is.EqualTo(99));
        }
    }
}
