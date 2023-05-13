using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Threading;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading
{
    [TestFixture]
    public class MonitorQueueTests
    {
        [Test]
        public void Initialize_Default()
        {
            var queue = new MonitorQueue<int>();
            Assert.That(queue.Count, Is.EqualTo(0));
            Assert.That(queue.SyncRoot, Is.Not.Null);
        }

        [Test]
        public void Initialize_Collection()
        {
            int[] numbers = { 1, 1, 2, 3, 5, 8, 13 };

            var queue = new MonitorQueue<int>(numbers);
            Assert.That(queue.Count, Is.EqualTo(numbers.Length));
            Assert.That(queue.SyncRoot, Is.Not.Null);
        }

        [Test]
        public void Enqueue()
        {
            var queue = new MonitorQueue<int>();

            queue.Enqueue(10);
            Assert.That(queue.Count, Is.EqualTo(1));
            Assert.That(queue.Peek(), Is.EqualTo(10));

            queue.Enqueue(20);
            Assert.That(queue.Count, Is.EqualTo(2));
            Assert.That(queue.Peek(), Is.EqualTo(10));
        }

        [Test]
        public void EnqueueCollection()
        {
            var queue = new MonitorQueue<int>();

            int[] numbers = { 3, 6, 9 };

            queue.Enqueue(numbers);
            Assert.That(queue.Count, Is.EqualTo(3));
        }

        [Test]
        public void TryDequeue_Empty()
        {
            var queue = new MonitorQueue<int>();

            int item;
            bool gotItem = queue.TryDequeue(out item);

            Assert.That(gotItem, Is.False);
            Assert.That(item, Is.EqualTo(default(int)));
        }

        [Test]
        public void TryDequeue_NotEmpty()
        {
            var queue = new MonitorQueue<int>();

            queue.Enqueue(8);
            queue.Enqueue(16);

            int item;
            bool gotItem = queue.TryDequeue(out item);

            Assert.That(gotItem, Is.True);
            Assert.That(item, Is.EqualTo(8));

            Assert.That(queue.Count, Is.EqualTo(1));
            Assert.That(queue.Peek(), Is.EqualTo(16));
        }

        [Test]
        public void TryDequeue_Wait_Empty()
        {
            var queue = new MonitorQueue<int>();

            int item;
            bool gotItem = queue.TryDequeue(TimeSpan.Zero, out item);

            Assert.That(gotItem, Is.False);
            Assert.That(item, Is.EqualTo(default(int)));
        }

        [Test]
        public void TryDequeue_Wait_NotEmpty()
        {
            var queue = new MonitorQueue<int>();
            queue.Enqueue(23);

            int item;
            bool gotItem = queue.TryDequeue(TimeSpan.Zero, out item);

            Assert.That(gotItem, Is.True);
            Assert.That(item, Is.EqualTo(23));

            queue.Enqueue(99);
            gotItem = queue.TryDequeue(TimeSpan.FromMilliseconds(20), out item);

            Assert.That(gotItem, Is.True);
            Assert.That(item, Is.EqualTo(99));
        }

        [Test]
        public void DequeueWithWait()
        {
            var queue = new MonitorQueue<int>();
            queue.Enqueue(23);

            var item = queue.DequeueWithWait();

            Assert.That(item, Is.EqualTo(23));
            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void TryPeek_Empty()
        {
            var queue = new MonitorQueue<int>();

            int item;
            bool gotItem = queue.TryPeek(out item);

            Assert.That(gotItem, Is.False);
            Assert.That(item, Is.EqualTo(default(int)));
        }

        [Test]
        public void TryPeek_NotEmpty()
        {
            var queue = new MonitorQueue<int>();

            queue.Enqueue(8);
            queue.Enqueue(16);

            int item;
            bool gotItem = queue.TryPeek(out item);

            Assert.That(gotItem, Is.True);
            Assert.That(item, Is.EqualTo(8));
            Assert.That(queue.Count, Is.EqualTo(2));
        }

        [Test]
        public void TryPeek_Wait_NotEmpty()
        {
            var queue = new MonitorQueue<int>();
            queue.Enqueue(23);

            int item;
            bool gotItem = queue.TryPeek(TimeSpan.Zero, out item);

            Assert.That(gotItem, Is.True);
            Assert.That(item, Is.EqualTo(23));
            Assert.That(queue.Count, Is.EqualTo(1));

            queue.Enqueue(99);
            gotItem = queue.TryPeek(TimeSpan.FromMilliseconds(20), out item);

            Assert.That(gotItem, Is.True);
            Assert.That(item, Is.EqualTo(23));
            Assert.That(queue.Count, Is.EqualTo(2));
        }

        [Test]
        public void Peek()
        {
            var queue = new MonitorQueue<int>();
            queue.Enqueue(58);

            Assert.That(queue.Peek(), Is.EqualTo(58));
        }

        [Test]
        public void SyncRoot()
        {
            var queue = new MonitorQueue<int>();
            Assert.That(queue.SyncRoot, Is.Not.Null);
        }

        [Test]
        public void Enumerator()
        {
            var queue = new MonitorQueue<int>();

            queue.Enqueue(1);
            queue.Enqueue(1);
            queue.Enqueue(2);

            var sum = queue.Sum();
            Assert.That(sum, Is.EqualTo(4));
        }
    }
}
