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
    public class AsyncSemaphoreTests
    {
        [Test]
        public void Initialization_BadCount()
        {
            Assert.Catch(() => new AsyncSemaphore(-1));
        }

        [Test]
        public void AcquireOne()
        {
            var semaphore = new AsyncSemaphore(1);

            Assert.That(semaphore.WaitAsync().Wait(500), Is.True);
            Assert.That(semaphore.WaitAsync().Wait(500), Is.False);
        }

        [Test]
        public async Task AcquireOne_Release()
        {
            var semaphore = new AsyncSemaphore(1);
            var flag = false;

            await semaphore.WaitAsync();

            var task = Task.Run(async () =>
            {
                await semaphore.WaitAsync();
                flag = true;
            });

            Assert.That(flag, Is.False);
            semaphore.Release();

            await task;
            Assert.That(flag, Is.True);
        }

        [Test]
        public async Task AcquireOne_Release_GetAwaiter()
        {
            var semaphore = new AsyncSemaphore(1);
            var flag = false;

            await semaphore.WaitAsync();

            var task = Task.Run(async () =>
            {
                await semaphore;
                flag = true;
            });

            Assert.That(flag, Is.False);
            semaphore.Release();

            await task;
            Assert.That(flag, Is.True);
        }

        [Test]
        public async Task AcquireMany_Release()
        {
            var semaphore = new AsyncSemaphore(2);
            var stepDoneHandle = new AsyncAutoResetEvent(false);

            var flag1 = false;
            var flag2 = false;

            await semaphore.WaitAsync();

            var task = Task.Run(async () =>
            {
                await semaphore;
                flag1 = true;
                stepDoneHandle.Set();

                await semaphore;
                flag2 = true;

            });

            await stepDoneHandle;

            Assert.That(flag1, Is.True);
            semaphore.Release();

            await task;
            Assert.That(flag2, Is.True);
        }
    }
}
