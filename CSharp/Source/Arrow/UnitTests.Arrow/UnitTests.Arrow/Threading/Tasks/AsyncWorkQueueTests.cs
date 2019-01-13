using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading.Tasks
{
    [TestFixture]
    public class AsyncWorkQueueTests
    {
        [Test]
        public async Task QueueID()
        {
            using(var queue = new AsyncWorkQueue())
            {
                Assert.That(queue.QueueID, Is.Not.EqualTo(AsyncWorkQueue.NoQueue));
                Assert.That(AsyncWorkQueue.ActiveQueueID, Is.EqualTo(AsyncWorkQueue.NoQueue));

                await queue.Enqueue(async () =>
                {
                    Assert.That(queue.QueueID, Is.Not.EqualTo(AsyncWorkQueue.NoQueue));
                    var id = queue.QueueID;

                    await Task.Yield();
                    Assert.That(queue.QueueID, Is.EqualTo(id));
                });

                Assert.That(queue.QueueID, Is.Not.EqualTo(AsyncWorkQueue.NoQueue));
                Assert.That(AsyncWorkQueue.ActiveQueueID, Is.EqualTo(AsyncWorkQueue.NoQueue));
            }
        }

        [Test]
        public async Task Enqueue_Action()
        {
            using(var queue = new AsyncWorkQueue())
            {
                long flag = 0;

                await queue.Enqueue(async () =>
                {
                    await Task.Yield();
                    Interlocked.Exchange(ref flag, 1);
                });

                Assert.That(Interlocked.Read(ref flag), Is.EqualTo(1));
            }
        }
    }
}
