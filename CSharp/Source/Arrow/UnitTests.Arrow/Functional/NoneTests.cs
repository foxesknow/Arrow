using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Functional;
using NUnit.Framework;

#nullable enable

namespace UnitTests.Arrow.Functional
{
    [TestFixture]
    public class NoneTests
    {
        [Test]
        public void AsString()
        {
            var none = new None();
            Assert.That(none.ToString(), Is.EqualTo("None"));
        }

        [Test]
        public void Hashing()
        {
            var none = new None();
            Assert.That(none.GetHashCode(), Is.EqualTo(0));
        }

        [Test]
        public void Equality()
        {
            var lhs = new None();
            var rhs = new None();

            Assert.That(lhs, Is.EqualTo(rhs));
            Assert.That(lhs.Equals(rhs), Is.True);
            Assert.That(lhs.Equals((object?)rhs), Is.True);
            Assert.That(lhs.Equals((object?)null), Is.False);
            Assert.That(lhs.Equals((object?)"hello"), Is.False);
        }

        [Test]
        public void Equality_Option()
        {
            var lhs = new None();

            Option<int> something = 10;
            Option<string> nothing = Option.None;

            Assert.That(lhs.Equals(something), Is.False);
            Assert.That(lhs.Equals(nothing), Is.True);

            Assert.That(lhs.Equals((object?)something), Is.False);
            Assert.That(lhs.Equals((object?)nothing), Is.True);
        }
    }
}
