using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Threading;
using Arrow.Threading.Tasks;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading.Tasks
{
    [TestFixture]
    public class SequentialWorkQueueTests
    {
        [Test]
        public void ScheduleOne()
        {
            long flag = 0;

            Func<ValueTask> setter = () =>
            {
                Interlocked.Increment(ref flag);
                return default;
            };

            using(var queue = new SequentialWorkQueue())
            {
                queue.Enqueue(setter);
            }

            Assert.That(Interlocked.Read(ref flag), Is.EqualTo(1));
        }

        [Test]
        public void ScheduleTwo()
        {
            long flag = 0;

            Func<ValueTask> add1 = () =>
            {
                Interlocked.Increment(ref flag);
                return default;
            };

            Func<ValueTask> add2 = () =>
            {
                Interlocked.Add(ref flag, 2);
                return default;
            };

            using(var queue = new SequentialWorkQueue())
            {
                queue.Enqueue(add1);
                queue.Enqueue(add2);
            }

            Assert.That(Interlocked.Read(ref flag), Is.EqualTo(3));
        }

        [Test]
        public void EnsureFirstCompletesBeforeSecond()
        {
            long flag = 10;
            long flagAsSeenByFirst = 0;

            Func<ValueTask> first = async () =>
            {
                await Task.Delay(1000).ContinueOnAnyContext();

                // We should read 10 as we're running first.
                // If "second" gets ahead of us then we've see 12
                var flagValue = Interlocked.Read(ref flag);
                Interlocked.Exchange(ref flagAsSeenByFirst, flagValue);

                Interlocked.Increment(ref flag);
            };

            Func<ValueTask> second = () =>
            {
                Interlocked.Add(ref flag, 2);
                return default;
            };

            using(var queue = new SequentialWorkQueue())
            {
                queue.Enqueue(first);
                queue.Enqueue(second);
            }

            Assert.That(Interlocked.Read(ref flag), Is.EqualTo(13));
            Assert.That(Interlocked.Read(ref flagAsSeenByFirst), Is.EqualTo(10));
        }

        [Test]
        public void LotsOfFunctions()
        {
            long counter = 0;

            Func<ValueTask> adder = () =>
            {
                Interlocked.Increment(ref counter);
                return default;
            };

            using(var stopEvent = new ManualResetEvent(false))
            {
                using(var queue = new SequentialWorkQueue())
                {
                    for(var i = 0; i < 1000; i++)
                    {
                        queue.Enqueue(adder);
                    }

                    queue.Enqueue(() =>
                    {
                        stopEvent.Set();
                        return default;
                    });

                    // Wait for everything to finish before doing the assertions
                    stopEvent.WaitOne();
                }
            }

            Assert.That(Interlocked.Read(ref counter), Is.EqualTo(1000));
        }

        [Test]
        public void ExceptionsAreIgnored()
        {
            long flag = 0;

            Func<ValueTask> add1 = () =>
            {
                Interlocked.Increment(ref flag);
                return default;
            };

            Func<ValueTask> add2 = () =>
            {
                Interlocked.Add(ref flag, 2);
                throw new Exception("bang!");
            };

            using(var queue = new SequentialWorkQueue())
            {
                queue.Enqueue(add1);
                queue.Enqueue(add2);
            }

            Assert.That(Interlocked.Read(ref flag), Is.EqualTo(3));
        }
    }
}
