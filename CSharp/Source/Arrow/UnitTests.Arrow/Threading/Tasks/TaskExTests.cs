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
    public class TaskExTests
    {
        [Test]
        public async Task SwitchToContext()
        {
            var testContext = SynchronizationContext.Current;

            using(var queue = new AsyncWorkQueue())
            {
                var task = queue.EnqueueAsync(async () =>
                {
                    // Initially we'll be on the queue context
                    var startContext = SynchronizationContext.Current;
                    Assert.That(startContext, Is.Not.Null);

                    await TaskEx.SwitchToContext(testContext);

                    var endContext = SynchronizationContext.Current;
                    Assert.That(endContext, Is.EqualTo(testContext));
                });


                await task;
            }
        }
    }
}
