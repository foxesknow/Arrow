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
    public class AsyncAutoResetEventTests
    {
        [Test]
        public async Task TestEvent()
        {
            using(var nextStepHandle = new AsyncAutoResetEvent(false))
            using(var stepDoneHandle = new AsyncAutoResetEvent(false))
            {
                var flag1 = false;
                var flag2 = false;
                var flag3 = false;

                var task = Task.Run(async () =>
                {
                    await nextStepHandle;
                    flag1 = true;
                    stepDoneHandle.Set();

                    await nextStepHandle;
                    flag2 = true;
                    stepDoneHandle.Set();

                    await nextStepHandle;
                    flag3 = true;
                    stepDoneHandle.Set();
                });

                Assert.That(flag1, Is.False);
                Assert.That(flag2, Is.False);
                Assert.That(flag3, Is.False);

                nextStepHandle.Set();
                await stepDoneHandle;

                Assert.That(flag1, Is.True);
                Assert.That(flag2, Is.False);
                Assert.That(flag3, Is.False);

                nextStepHandle.Set();
                await stepDoneHandle;

                Assert.That(flag1, Is.True);
                Assert.That(flag2, Is.True);
                Assert.That(flag3, Is.False);

                nextStepHandle.Set();
                await stepDoneHandle;

                Assert.That(flag1, Is.True);
                Assert.That(flag2, Is.True);
                Assert.That(flag3, Is.True);

                await task;
            }
        }
    }
}
