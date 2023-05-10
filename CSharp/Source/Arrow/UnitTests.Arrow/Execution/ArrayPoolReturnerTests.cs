using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

using NUnit.Framework;

namespace UnitTests.Arrow.Execution
{
    [TestFixture]
    public class ArrayPoolReturnerTests
    {
        [Test]
        public void Initialization_NullBuffer()
        {
            var pool = ArrayPool<int>.Shared;

            Assert.Catch(() => new ArrayPoolReturner<int>(pool, null, 0, 10));
            Assert.Catch(() => new ArrayPoolReturner<int>(pool, null, 10, 0));
        }

        [Test]
        public void Initialization_NegativeRangeValues()
        {
            var pool = ArrayPool<int>.Shared;
            var buffer = pool.Rent(20);

            Assert.Catch(() => new ArrayPoolReturner<int>(pool, buffer, -1, 10));
            Assert.Catch(() => new ArrayPoolReturner<int>(pool, buffer, 0, -5));
        }

        [Test]
        public void Initialization_InvalidRange()
        {
            var pool = ArrayPool<int>.Shared;
            var buffer = pool.Rent(20);

            Assert.Catch(() => new ArrayPoolReturner<int>(pool, buffer, 0, buffer.Length + 1));
            Assert.Catch(() => new ArrayPoolReturner<int>(pool, buffer, 0, buffer.Length + 10));
            Assert.Catch(() => new ArrayPoolReturner<int>(pool, buffer, 10, buffer.Length + 11));
            Assert.Catch(() => new ArrayPoolReturner<int>(pool, buffer, 10, buffer.Length + 20));
        }

        [Test]
        public void DefaultInitialization()
        {
            ArrayPoolReturner<byte> returner = default;
            Assert.That(returner.HasBuffer, Is.False);
            Assert.That(returner.Start, Is.EqualTo(0));
            Assert.That(returner.Length, Is.EqualTo(0));
            Assert.That(returner.Buffer, Is.Null);
        }

        [Test]
        public void ZeroLength()
        {
            var pool = ArrayPool<int>.Shared;
            var buffer = pool.Rent(0);

            using(var returner = new ArrayPoolReturner<int>(pool, buffer, 0, 0))
            {
                Assert.That(returner.HasBuffer, Is.True);
                Assert.That(returner.Start, Is.EqualTo(0));
                Assert.That(returner.Length, Is.EqualTo(0));
                Assert.That(returner.Buffer, Is.Not.Null);
                Assert.That(returner.Buffer, Is.SameAs(buffer));
            }
        }

        [Test]
        public void NoOffset()
        {
            var pool = ArrayPool<int>.Shared;
            var buffer = pool.Rent(20);

            using(var returner = new ArrayPoolReturner<int>(pool, buffer, 0, 20))
            {
                Assert.That(returner.HasBuffer, Is.True);
                Assert.That(returner.Start, Is.EqualTo(0));
                Assert.That(returner.Length, Is.EqualTo(20));
                Assert.That(returner.Buffer, Is.Not.Null);
                Assert.That(returner.Buffer, Is.SameAs(buffer));

                var span = returner.AsSpan();
                Assert.That(span.Length, Is.EqualTo(20));

                var memory = returner.AsMemory();
                Assert.That(memory.Length, Is.EqualTo(20));
            }
        }

        [Test]
        public void NonZeroOffset()
        {
            var pool = ArrayPool<int>.Shared;
            var buffer = pool.Rent(20);

            using(var returner = new ArrayPoolReturner<int>(pool, buffer, 6, 13))
            {
                Assert.That(returner.HasBuffer, Is.True);
                Assert.That(returner.Start, Is.EqualTo(6));
                Assert.That(returner.Length, Is.EqualTo(13));
                Assert.That(returner.Buffer, Is.Not.Null);
                Assert.That(returner.Buffer, Is.SameAs(buffer));

                var span = returner.AsSpan();
                Assert.That(span.Length, Is.EqualTo(13));

                var memory = returner.AsMemory();
                Assert.That(memory.Length, Is.EqualTo(13));
            }
        }
    }
}
