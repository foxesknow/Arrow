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
                Assert.That(queue.ID, Is.Not.EqualTo(AsyncWorkQueue.NoActiveQueue));
                Assert.That(AsyncWorkQueue.ActiveID, Is.EqualTo(AsyncWorkQueue.NoActiveQueue));

                await queue.EnqueueAsync(async () =>
                {
                    Assert.That(queue.ID, Is.Not.EqualTo(AsyncWorkQueue.NoActiveQueue));
                    var id = queue.ID;

                    await Task.Yield();
                    Assert.That(queue.ID, Is.EqualTo(id));
                });

                Assert.That(queue.ID, Is.Not.EqualTo(AsyncWorkQueue.NoActiveQueue));
                Assert.That(AsyncWorkQueue.ActiveID, Is.EqualTo(AsyncWorkQueue.NoActiveQueue));
            }
        }

        [Test]
        public async Task Enqueue_Action()
        {
            using(var queue = new AsyncWorkQueue())
            {
                long flag = 0;

                await queue.EnqueueAsync(async () =>
                {
                    await Task.Yield();
                    Interlocked.Exchange(ref flag, 1);
                    return 1;
                });

                Assert.That(Interlocked.Read(ref flag), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task TryEnqueue_Func_Task_Int()
        {
            using(var queue = new AsyncWorkQueue())
            {
                var enqueued = queue.TryEnqueueAsync(async () =>
                {
                    await Task.Yield();
                    return 1;
                }, out var task);

                if(enqueued)
                {
                    var value = await task;
                    Assert.That(value, Is.EqualTo(1));
                }
                else
                {
                    Assert.Fail("call should have been enqueued");
                }                
            }
        }
    }
}
