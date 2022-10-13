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
    public class AsyncManualResetEventTests
    {
        [Test]
        public async Task SetAndWait()
        {
            var evt = new AsyncManualResetEvent(false);

            evt.Set();
            await evt.WaitAsync();

            Assert.IsTrue(true);
        }

        [Test]
        public async Task SetAndWait_Thread_MultipleWaits()
        {
            var evt = new AsyncManualResetEvent(false);

            var t = Task.Run(async () =>
            {
                await Task.Delay(500);
                evt.Set();
            });

            // We can wait multiple times as the event doesn't reset
            await evt.WaitAsync();
            await evt.WaitAsync();
            await evt.WaitAsync();

            Assert.IsTrue(true);
        }
    }
}
