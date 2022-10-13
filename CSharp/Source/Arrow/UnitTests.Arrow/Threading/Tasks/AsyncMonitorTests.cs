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
    public class AsyncMonitorTests
    {
        [Test]
        public async Task LockOnMultipleThreads()
        {
            int x = 0;
            int y = 0;

            var monitor = new AsyncMonitor();
            var barrier = new AsyncBarrier(4);

            async Task ThreadMain()
            {
                // Queue up the threads 
                await barrier.SignalAndWait();

                using(await monitor.LockAsync())
                {
                    if(x == 0) Assert.That(y, Is.EqualTo(0));
                    if(x == 1) Assert.That(y, Is.EqualTo(1));
                    if(x == 2) Assert.That(y, Is.EqualTo(2));
                    if(x == 3) Assert.That(y, Is.EqualTo(3));

                    x++;
                    y++;
                }
            }

            await Task.WhenAll
            (
                Task.Run(ThreadMain),
                Task.Run(ThreadMain),
                Task.Run(ThreadMain),
                Task.Run(ThreadMain)
            );


            using(await monitor.LockAsync())
            {
                Assert.That(x, Is.EqualTo(4));
                Assert.That(y, Is.EqualTo(4));
            }
        }
    }
}
