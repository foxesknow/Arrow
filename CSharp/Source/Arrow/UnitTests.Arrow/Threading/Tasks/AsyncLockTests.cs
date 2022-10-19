using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading.Tasks
{
    [TestFixture]
    public class AsyncLockTests
    {
        [Test]
        public void TryLock()
        {
            var syncRoot = new AsyncLock();

            using(syncRoot.TryLock(out var lockAcquired))
            {
                Assert.That(lockAcquired, Is.True);
            }

            using(syncRoot.TryLock(out var lockAcquired))
            {
                Assert.That(lockAcquired, Is.True);
            }
        }

        [Test]
        public async Task TryLock_AlreadyAcquired()
        {
            var syncRoot = new AsyncLock();

            using(await syncRoot)
            {
                using(syncRoot.TryLock(out var lockAcquired))
                {
                    Assert.That(lockAcquired, Is.False);
                }
            }

            using(syncRoot.TryLock(out var lockAcquired))
            {
                Assert.That(lockAcquired, Is.True);
            }
        }

        [Test]
        public async Task SameThread_MultipleLocks()
        {
            var syncRoot = new AsyncLock();
            var count = 0;

            using(await syncRoot)
            {
                count++;
            }

            using(await syncRoot)
            {
                count++;
            }

            Assert.That(count, Is.EqualTo(2));
        }

        [Test]
        public async Task MultipleThreads()
        {
            var syncRoot = new AsyncLock();
            var goEvent = new AsyncManualResetEvent(false);

            var reps = 100_000;
            var count = 0;

            Func<Task> thread = async () =>
            {
                await goEvent;

                for(var i = 0; i < reps; i++)
                {
                    using(await syncRoot)
                    {
                        count++;
                    }
                }
            };

            var tasks = new Task[]
            {
                Task.Run(thread),
                Task.Run(thread),
                Task.Run(thread),
                Task.Run(thread),
            };

            // Give the threads time to get up and running
            await Task.Delay(500);

            // Set them off...
            goEvent.Set();

            // ...and wait for them to finish
            await Task.WhenAll(tasks);

            using(await syncRoot)
            {
                Assert.That(count, Is.EqualTo(reps * tasks.Length));
            }
        }
    }
}
