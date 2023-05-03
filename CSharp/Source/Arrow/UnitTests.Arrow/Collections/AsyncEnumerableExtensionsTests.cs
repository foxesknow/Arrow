using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
    [TestFixture]
    public class AsyncEnumerableExtensionsTests
    {
        [Test]
        public void InvalidSequence()
        {
            IEnumerable<int> numbers = null;
            Assert.Catch(() => numbers.ToAsyncEnumerable());
        }

        [Test]
        public async Task Enumerate()
        {
            var sequence = Enumerable.Range(0, 10).ToAsyncEnumerable();
            Assert.That(sequence, Is.Not.Null);

            var expected = 0;

            await foreach(var number in sequence)
            {
                Assert.That(number, Is.EqualTo(expected));
                expected++;
            }
        }

        [Test]
        public async Task CanBeCancelled()
        {
            using(var cts = new CancellationTokenSource())
            {
                var sequence = Enumerable.Range(0, 10).ToAsyncEnumerable();
                var lastSeen = 0;

                Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    await foreach(var number in sequence.WithCancellation(cts.Token))
                    {
                        lastSeen = number;

                        if(number == 5) cts.Cancel();
                    }
                });

                Assert.That(lastSeen, Is.EqualTo(5));
            }
        }
    }
}
