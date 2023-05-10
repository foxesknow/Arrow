using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut
{
    [TestFixture]
    public class OrdinalItemTests
    {
        [Test]
        public void FromParams()
        {
            var list = OrdinalItem.AsList("Jack", "Ben", "Sawyer");
            Assert.That(list.Count, Is.EqualTo(3));

            Assert.That(list[0].Name, Is.EqualTo("Jack"));
            Assert.That(list[0].Ordinal, Is.EqualTo(0));

            Assert.That(list[1].Name, Is.EqualTo("Ben"));
            Assert.That(list[1].Ordinal, Is.EqualTo(1));

            Assert.That(list[2].Name, Is.EqualTo("Sawyer"));
            Assert.That(list[2].Ordinal, Is.EqualTo(2));
        }

        [Test]
        public void FromList()
        {
            var list = OrdinalItem.AsList(new List<string>(){"Jack", "Ben", "Sawyer"});
            Assert.That(list.Count, Is.EqualTo(3));

            Assert.That(list[0].Name, Is.EqualTo("Jack"));
            Assert.That(list[0].Ordinal, Is.EqualTo(0));

            Assert.That(list[1].Name, Is.EqualTo("Ben"));
            Assert.That(list[1].Ordinal, Is.EqualTo(1));

            Assert.That(list[2].Name, Is.EqualTo("Sawyer"));
            Assert.That(list[2].Ordinal, Is.EqualTo(2));
        }

        [Test]
        public void FromEnum()
        {
            var list = OrdinalItem.AsList<Names>();
            Assert.That(list.Count, Is.EqualTo(2));

            Assert.That(list[0].Name, Is.EqualTo("Kate"));
            Assert.That(list[0].Ordinal, Is.EqualTo(5));

            Assert.That(list[1].Name, Is.EqualTo("Juliet"));
            Assert.That(list[1].Ordinal, Is.EqualTo(9));

        }

        private enum Names
        {
            Kate = 5,
            Juliet = 9
        }
    }
}
