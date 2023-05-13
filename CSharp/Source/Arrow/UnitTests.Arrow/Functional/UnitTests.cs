using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Functional;
using NUnit.Framework;

namespace UnitTests.Arrow.Functional
{
    [TestFixture]
    public class UnitTests
    {
        [Test]
        public void Equality()
        {
            Unit lhs = new();
            Unit rhs = new();

            Assert.That(lhs.Equals(rhs), Is.True);
            Assert.That(lhs.Equals((object)rhs), Is.True);

            Assert.That(lhs.Equals(null), Is.False);
        }

        [Test]
        public void Hashing()
        {
            Unit lhs = new();
            Unit rhs = new();

            Assert.That(lhs.GetHashCode(), Is.EqualTo(rhs.GetHashCode()));
        }

        [Test]
        public void AsString()
        {
            Unit lhs = new();
            Assert.That(lhs.ToString(), Is.Not.Null & Has.Length.GreaterThan(0));
        }

        [Test]
        public void Compare()
        {
            Unit lhs = new();
            Unit rhs = new();

            Assert.That(lhs.CompareTo(rhs), Is.EqualTo(0));
        }
    }
}
