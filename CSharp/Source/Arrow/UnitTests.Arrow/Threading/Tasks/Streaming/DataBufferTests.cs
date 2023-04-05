using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;
using Arrow.Threading.Tasks.Streaming;
using NUnit.Framework;

namespace UnitTests.Arrow.Threading.Tasks.Streaming
{
    [TestFixture]
    public class DataBufferTests
    {
        [Test]
        public void TryPeek_EmptyBuffer()
        {
            var buffer = new DataBuffer<string>();
            Assert.That(buffer.TryPeek(out var _, static s => true), Is.False);
        }

        [Test]
        public async Task TryPeek()
        {
            var buffer = new DataBuffer<string>();
            buffer.Publish("Jack");

            Assert.That(buffer.TryPeek(out var name, static s => true), Is.True);
            Assert.That(name, Is.EqualTo("Jack"));

            Assert.That(buffer.TryPeek(out var name2, static s => true), Is.True);
            Assert.That(name2, Is.EqualTo("Jack"));

            var data = await buffer.WaitFor(static s => true);
            Assert.That(data, Is.EqualTo("Jack"));
        }

        [Test]
        public async Task TryPeek_NotPresent()
        {
            var buffer = new DataBuffer<string>();
            buffer.Publish("Jack");

            Assert.That(buffer.TryPeek(out var _, static s => s == "Ben"), Is.False);

            var data = await buffer.WaitFor(static s => true);
            Assert.That(data, Is.EqualTo("Jack"));
        }

        [Test]
        public async Task Clear()
        {
            var buffer = new DataBuffer<string>();
            buffer.Publish("Jack");
            buffer.Publish("Ben");

            Assert.That(await buffer.PeekFor(static s => s == "Jack"), Is.EqualTo("Jack"));

            buffer.Clear();

            Assert.ThrowsAsync<TimeoutException>(async () => await buffer.PeekFor(TimeSpan.Zero, static s => s == "Ben"));
            Assert.ThrowsAsync<TimeoutException>(async () => await buffer.PeekFor(TimeSpan.Zero, static s => s == "Jack"));
        }

        [Test]
        public void CancelAll()
        {
            var buffer = new DataBuffer<string>();
            var task1 = buffer.WaitFor(static s => s == "Ben");
            var task2 = buffer.WaitFor(static s => s == "Jack");
            var task3 = buffer.WaitFor(static s => s == "Sawyer");

            buffer.CancelAll();

            Assert.CatchAsync<OperationCanceledException>(async () => await task1);
            Assert.CatchAsync<OperationCanceledException>(async () => await task2);
            Assert.CatchAsync<OperationCanceledException>(async () => await task3);
        }

        [Test]
        public void Cancel()
        {
            var buffer = new DataBuffer<string>();
            var task1 = buffer.WaitFor(static s => s == "Ben");
            Assert.That(buffer.Cancel(task1), Is.True);

            buffer.Publish("Ben");

            Assert.CatchAsync<OperationCanceledException>(() => task1);
            Assert.That(buffer.HasData, Is.True);
        }

        [Test]
        public async Task Cancel_Multiple_With_Same_Condition()
        {
            var buffer = new DataBuffer<string>();
            var wait1 = buffer.WaitFor(static s => s == "Ben");
            var wait2 = buffer.WaitFor(static s => s == "Ben");
            var peek1 = buffer.PeekFor(static s => s == "Ben");
            var peek2 = buffer.PeekFor(static s => s == "Ben");

            Assert.That(buffer.Cancel(wait1), Is.True);
            Assert.That(buffer.Cancel(peek2), Is.True);

            buffer.Publish("Ben");
            Assert.CatchAsync<OperationCanceledException>(async () => await wait1);
            Assert.CatchAsync<OperationCanceledException>(async () => await peek2);

            Assert.That(await wait2, Is.EqualTo("Ben"));
            Assert.That(await peek1, Is.EqualTo("Ben"));

            Assert.That(buffer.HasData, Is.False);
        }

        [Test]
        public async Task Cancel_After_Data()
        {
            var buffer = new DataBuffer<string>();

            var wait1 = buffer.WaitFor(static s => s == "Ben");
            buffer.Publish("Ben");

            Assert.That(buffer.Cancel(wait1), Is.False);
            Assert.That(await wait1, Is.EqualTo("Ben"));
        }

        [Test]
        public async Task WaitMultiple()
        {
            var buffer = new DataBuffer<string>();

            var wait1 = buffer.WaitFor(static s => s == "Ben");
            var wait2 = buffer.WaitFor(static s => s == "Ben");

            buffer.Publish("Jack");
            buffer.Publish("Ben");

            await Task.WhenAll(wait1, wait2);

            Assert.That(await wait1, Is.EqualTo("Ben"));
            Assert.That(await wait2, Is.EqualTo("Ben"));

            // Jack is still in the buffer
            Assert.That(buffer.HasData, Is.True);
        }

        [Test]
        public async Task WaitMultiple_SameDataItem()
        {
            var buffer = new DataBuffer<string>();

            var wait1 = buffer.WaitFor(static s => s == "Ben");
            var wait2 = buffer.WaitFor(static s => s == "Ben");

            buffer.Publish("Ben");
            buffer.Publish("Ben");

            await Task.WhenAll(wait1, wait2);

            Assert.That(await wait1, Is.EqualTo("Ben"));
            Assert.That(await wait2, Is.EqualTo("Ben"));

            // There's still a Ben in the buffer
            Assert.That(buffer.HasData, Is.True);
        }

        [Test]
        public async Task WaitThenPublish()
        {
            var buffer = new DataBuffer<string>();

            var waitForBenTask = buffer.WaitFor(static s => s == "Ben");
            buffer.Publish("Jack");
            buffer.Publish("Ben");

            Assert.That(await waitForBenTask, Is.EqualTo("Ben"));
        }

        [Test]
        public async Task WaitFor_ZeroTimeout_1()
        {
            var buffer = new DataBuffer<string>();

            buffer.Publish("Jack");
            buffer.Publish("Ben");

            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Ben"), Is.EqualTo("Ben"));
            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Jack"), Is.EqualTo("Jack"));
        }

        [Test]
        public async Task WaitFor_ZeroTimeout_2()
        {
            var buffer = new DataBuffer<string>();

            buffer.Publish("Ben");
            buffer.Publish("Jack");

            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Ben"), Is.EqualTo("Ben"));
            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Jack"), Is.EqualTo("Jack"));
        }

        [Test]
        public async Task WaitFor_ZeroTimeout_Action_1()
        {
            var buffer = new DataBuffer<string>();

            buffer.Publish("Ben");
            buffer.Publish("Jack");

            var gotBen = false;
            var gotJack = false;

            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Ben", s => gotBen = true), Is.EqualTo("Ben"));
            Assert.That(gotBen, Is.True);

            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Jack", s => gotJack = true), Is.EqualTo("Jack"));
            Assert.That(gotJack, Is.True);
        }

        [Test]
        public async Task WaitFor_ZeroTimeout_Action_2()
        {
            var buffer = new DataBuffer<string>();

            buffer.Publish("Ben");
            buffer.Publish("Jack");

            var gotBen = false;
            var gotJack = false;

            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Jack", s => gotJack = true), Is.EqualTo("Jack"));
            Assert.That(gotJack, Is.True);

            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Ben", s => gotBen = true), Is.EqualTo("Ben"));
            Assert.That(gotBen, Is.True);
        }

        [Test]
        public async Task Wait_Timeout_DataAwaitable()
        {
            var buffer = new DataBuffer<string>();

            var publishTask = Task.Run(async () =>
            {
                await Task.Delay(1000);
                buffer.Publish("Jack");
            });

            Assert.That(await buffer.WaitFor(TimeSpan.FromSeconds(10), static s => s == "Jack"), Is.EqualTo("Jack"));
            await publishTask;
        }

        [Test]
        public void Wait_Timeout_DataNotAwaitable()
        {
            var buffer = new DataBuffer<string>();
            Assert.ThrowsAsync<TimeoutException>(async () => await buffer.WaitFor(TimeSpan.FromSeconds(1), static s => s == "Jack"));
        }

        [Test]
        public async Task PeekFor_1()
        {
            var buffer = new DataBuffer<string>();

            buffer.Publish("Jack");
            buffer.Publish("Ben");

            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Ben"), Is.EqualTo("Ben"));
            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Jack"), Is.EqualTo("Jack"));

            // As we peeked the data should still be there

            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Ben"), Is.EqualTo("Ben"));
            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Jack"), Is.EqualTo("Jack"));
        }

        [Test]
        public async Task PeekFor_2()
        {
            var buffer = new DataBuffer<string>();

            buffer.Publish("Ben");
            buffer.Publish("Jack");

            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Ben"), Is.EqualTo("Ben"));
            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Jack"), Is.EqualTo("Jack"));

            Assert.That(buffer.HasData, Is.True);
        }

        [Test]
        public async Task PeekForMultiple()
        {
            var buffer = new DataBuffer<string>();

            var peek1 = buffer.PeekFor(static s => s == "Ben");
            var peek2 = buffer.PeekFor(static s => s == "Ben");

            buffer.Publish("Jack");
            buffer.Publish("Ben");

            await Task.WhenAll(peek1, peek2);

            Assert.That(await peek1, Is.EqualTo("Ben"));
            Assert.That(await peek2, Is.EqualTo("Ben"));
        }

        [Test]
        public async Task PeekFor_After_WaitFor()
        {
            var buffer = new DataBuffer<string>();

            buffer.Publish("Ben");
            buffer.Publish("Jack");

            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Ben"), Is.EqualTo("Ben"));
            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Jack"), Is.EqualTo("Jack"));

            Assert.That(buffer.HasData, Is.True);

            Assert.That(await buffer.WaitFor(TimeSpan.Zero, static s => s == "Ben"), Is.EqualTo("Ben"));
            Assert.That(await buffer.PeekFor(TimeSpan.Zero, static s => s == "Jack"), Is.EqualTo("Jack"));
        }

        [Test]
        public async Task WaitFor_And_PeekFor()
        {
            var buffer = new DataBuffer<string>();

            var waitTask = buffer.WaitFor(static s => s == "Ben");
            var peekTask = buffer.WaitFor(static s => s == "Ben");

            buffer.Publish("Ben");

            Assert.That(await waitTask, Is.EqualTo("Ben"));
            Assert.That(await peekTask, Is.EqualTo("Ben"));

            Assert.That(buffer.HasData, Is.False);
        }

        [Test]
        public async Task StartActivity()
        {
            var buffer = new DataBuffer<string>();
            buffer.Publish("Ben");

            using (buffer.StartActivity())
            {
                Assert.That(buffer.HasData, Is.False);

                var publishTask = Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    buffer.Publish("Jack");
                    buffer.Publish("Sawyer");
                    buffer.Publish("Kate");
                });

                Assert.That(await buffer.WaitFor(TimeSpan.FromSeconds(10), s => s == "Kate"), Is.EqualTo("Kate"));
                Assert.That(await buffer.WaitFor(TimeSpan.FromSeconds(10), s => s == "Sawyer"), Is.EqualTo("Sawyer"));

                await publishTask;

                // Jack is still there
                Assert.That(buffer.HasData, Is.True);
            }

            // The Dispose will remove any data
            Assert.That(buffer.HasData, Is.False);
        }

        [Test]
        public async Task StartActivity_PendingAwait()
        {
            var buffer = new DataBuffer<string>();
            buffer.Publish("Ben2");

            Task hurleyTask = null;

            using (buffer.StartActivity())
            {
                Assert.That(buffer.HasData, Is.False);

                var publishTask = Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    buffer.Publish("Jack");
                    buffer.Publish("Sawyer");
                    buffer.Publish("Kate");
                });

                Assert.That(await buffer.WaitFor(TimeSpan.FromSeconds(10), s => s == "Kate"), Is.EqualTo("Kate"));
                Assert.That(await buffer.WaitFor(TimeSpan.FromSeconds(10), s => s == "Sawyer"), Is.EqualTo("Sawyer"));

                await publishTask;

                hurleyTask = buffer.WaitFor(s => s == "Hurley");
            }

            // The Dispose will cancel any pending tasks
            Assert.CatchAsync<OperationCanceledException>(async () => await hurleyTask);
        }

        [Test]
        public async Task StartActivity_MultiplePendingAwait()
        {
            var buffer = new DataBuffer<string>();
            buffer.Publish("Ben2");

            Task hurleyTask = null;
            Task benTask = null;
            Task lockeTask = null;

            using (buffer.StartActivity())
            {
                Assert.That(buffer.HasData, Is.False);

                var publishTask = Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    buffer.Publish("Jack");
                    buffer.Publish("Sawyer");
                    buffer.Publish("Kate");
                });

                Assert.That(await buffer.WaitFor(TimeSpan.FromSeconds(10), s => s == "Kate"), Is.EqualTo("Kate"));
                Assert.That(await buffer.WaitFor(TimeSpan.FromSeconds(10), s => s == "Sawyer"), Is.EqualTo("Sawyer"));

                await publishTask;

                hurleyTask = buffer.WaitFor(s => s == "Hurley");
                benTask = buffer.WaitFor(s => s == "Ben");
                lockeTask = buffer.WaitFor(s => s == "Locke");
            }

            // The Dispose will cancel any pending tasks
            Assert.CatchAsync<OperationCanceledException>(async () => await hurleyTask);
            Assert.CatchAsync<OperationCanceledException>(async () => await benTask);
            Assert.CatchAsync<OperationCanceledException>(async () => await lockeTask);
        }

        [Test]
        public void WaitFor_Throws()
        {
            var buffer = new DataBuffer<int>();
            buffer.Publish(0);

            Assert.CatchAsync(() => buffer.PeekFor(i => 10 / i == 2));
        }

        [Test]
        public void PeekFor_Throws()
        {
            var buffer = new DataBuffer<int>();
            buffer.Publish(0);

            Assert.CatchAsync(() => buffer.PeekFor(i => 10 / i == 2));
        }
    }
}
