using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution.Hashing;

using NUnit.Framework;

namespace UnitTests.Arrow.Execution.Hashing
{
    [TestFixture]
    public class FNV1ATests
    {
        [Test]
        public void DefaultInitialization()
        {
            FNV1A hasher = default;
            Assert.That(hasher.GetHashCode(), Is.EqualTo(0));
            Assert.That(hasher.HashValue, Is.EqualTo(0));
        }

        [Test]
        public void Begin()
        {
            // This is the FNV1a offset basis for 32bit hashing
            var offsetBasis = unchecked((int)0x811c9dc5);

            FNV1A hasher = new();
            hasher.Begin();
            Assert.That(hasher.GetHashCode(), Is.EqualTo(offsetBasis));
        }

        [Test]
        public void End()
        {
            // This is the FNV1a offset basis for 32bit hashing
            var offsetBasis = unchecked((int)0x811c9dc5);

            FNV1A hasher = new();
            hasher.Begin();
            Assert.That(hasher.HashValue, Is.EqualTo(offsetBasis));
        }

        [Test]
        public void HashesAreEqual_IntegerTypes()
        {
            var random = new Random();

            for(int i = 0; i < 100; i ++)
            {
                var value = random.Next(int.MinValue, int.MaxValue);

                var hasher1 = FNV1A.Make();
                hasher1.Begin();
                hasher1.Apply((byte)value);
                hasher1.Apply((short)value);
                hasher1.Apply((int)value);
                hasher1.Apply((long)value);

                var hasher2 = FNV1A.Make();
                hasher2.Begin();
                hasher2.Apply((byte)value);
                hasher2.Apply((short)value);
                hasher2.Apply((int)value);
                hasher2.Apply((long)value);

                Assert.That(hasher1.HashValue, Is.EqualTo(hasher2.HashValue));
            }
        }

        [Test]
        public void HashesAreEqual_FloatTypes()
        {
            var random = new Random();

            for(int i = 0; i < 100; i ++)
            {
                var doubleValue = random.NextDouble() * 10_000;
                var floatValue = random.NextDouble() * 10_000;

                var hasher1 = FNV1A.Make();
                hasher1.Begin();
                hasher1.Apply(doubleValue);
                hasher1.Apply(floatValue);

                var hasher2 = FNV1A.Make();
                hasher2.Begin();
                hasher2.Apply(doubleValue);
                hasher2.Apply(floatValue);

                Assert.That(hasher1.HashValue, Is.EqualTo(hasher2.HashValue));
            }
        }

        [Test]
        public void SmallValues()
        {
            var x = FNV1A.Make().Apply((byte)0);
            var y = FNV1A.Make().Apply((byte)1);
            var z = FNV1A.Make().Apply((byte)2);

            Assert.That(x, Is.Not.EqualTo(y));
            Assert.That(x, Is.Not.EqualTo(z));
            Assert.That(y, Is.Not.EqualTo(z));
        }
    }
}
