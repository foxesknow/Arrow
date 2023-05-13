using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections
{
    [TestFixture]
    public class RollingQueueTests
    {
        [Test]
        public void TestEnqueue()
        {
            RollingQueue<int> q = new RollingQueue<int>(5);
            Assert.IsTrue(q.MaxCount == 5);
            Assert.IsEmpty(q);

            q.Enqueue(123);
            Assert.IsTrue(q.Count == 1);

            q.Enqueue(234);
            Assert.IsTrue(q.Count == 2);

            q.Enqueue(345);
            q.Enqueue(456);
            q.Enqueue(567);

            Assert.IsTrue(q.Count == 5);
            Assert.IsTrue(q.Peek() == 123);

            // By adding one more item we will roll the queue.
            // This should remove 123 and add 678
            q.Enqueue(678);
            Assert.IsTrue(q.Count == 5);
            Assert.IsTrue(q.Peek() == 234);
        }

        [Test]
        public void TestDequeue()
        {
            RollingQueue<int> q = new RollingQueue<int>(5);

            q.Enqueue(1);
            q.Enqueue(2);
            q.Enqueue(3);

            Assert.IsTrue(q.Count == 3);

            Assert.IsTrue(q.Dequeue() == 1);
            Assert.IsTrue(q.Count == 2);

            Assert.IsTrue(q.Dequeue() == 2);
            Assert.IsTrue(q.Count == 1);

            Assert.IsTrue(q.Dequeue() == 3);
            Assert.IsTrue(q.Count == 0);
        }

        [Test]
        public void TestClear()
        {
            RollingQueue<int> q = new RollingQueue<int>(5);
            q.Clear();
            Assert.IsEmpty(q);

            q.Enqueue(20);
            Assert.IsNotEmpty(q);

            q.Clear();
            Assert.IsEmpty(q);
        }

        [Test]
        public void TestEnumeration()
        {
            RollingQueue<int> q = new RollingQueue<int>(5);
            q.Enqueue(1);
            q.Enqueue(2);
            q.Enqueue(3);
            q.Enqueue(4);

            int sum = 0;

            foreach(int i in q)
            {
                sum += i;
            }

            Assert.IsTrue(sum == 10);
        }

        [Test]
        public void TestPeek()
        {
            RollingQueue<int> q = new RollingQueue<int>(5);

            q.Enqueue(1);
            Assert.IsTrue(q.Peek() == 1);

            q.Enqueue(2);
            Assert.IsTrue(q.Peek() == 1);
        }

        [Test]
        public void TestDequeException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                RollingQueue<int> q = new RollingQueue<int>(5);
                q.Dequeue(); // This will fail
            });
        }

        [Test]
        public void TestPeekException()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                RollingQueue<int> q = new RollingQueue<int>(5);
                q.Peek(); // This will fail
            });
        }
    }
}
