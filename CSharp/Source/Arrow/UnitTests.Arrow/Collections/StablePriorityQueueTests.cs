using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Collections
{
    [TestFixture]
    public class StablePriorityQueueTests : PriorityQueueTestsBase
    {
        [Test]
        public void StableAdd_1()
        {
            var queue = CreateIntegerQueue();

            for(int i = 0; i < 10; i++)
            {
                queue.Enqueue(Priority.Medium, i);
            }

            Assert.That(queue.Count, Is.EqualTo(10));

            for(int i = 0; i < 10; i++)
            {
                var value = queue.Dequeue();
                Assert.That(value, Is.EqualTo(i));
            }

            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void StabeAdd_2()
        {
            var queue = CreateStringQueue();

            queue.Enqueue(Priority.Low, "Jack");
            queue.Enqueue(Priority.High, "Ben");
            queue.Enqueue(Priority.Medium, "Locke");
            queue.Enqueue(Priority.High, "Jacob");
            queue.Enqueue(Priority.Low, "Sawyer");

            Assert.That(queue.Dequeue(), Is.EqualTo("Ben"));
            Assert.That(queue.Dequeue(), Is.EqualTo("Jacob"));
            Assert.That(queue.Dequeue(), Is.EqualTo("Locke"));
            Assert.That(queue.Dequeue(), Is.EqualTo("Jack"));
            Assert.That(queue.Dequeue(), Is.EqualTo("Sawyer"));

            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void Enumeration()
        {
            var queue = CreateIntegerQueue();

            queue.Enqueue(Priority.Low, 2);
            queue.Enqueue(Priority.Medium, 1);
            queue.Enqueue(Priority.High, 0);

            int expected = 0;

            foreach(var pair in queue)
            {
                Assert.That(pair.Value, Is.EqualTo(expected));
                expected++;
            }
        }

        protected override IPriorityQueue<Priority, string> CreateStringQueue()
        {
            return new StablePriorityQueue<Priority, string>();
        }

        protected override IPriorityQueue<Priority, int> CreateIntegerQueue()
        {
            return new StablePriorityQueue<Priority, int>();
        }
    }
}
