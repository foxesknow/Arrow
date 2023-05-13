using System;
//using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Collections
{
    public abstract class PriorityQueueTestsBase
    {
        [Test]
        public void AddOne()
        {
            var queue = new PriorityQueue<Priority, string>();

            queue.Enqueue(Priority.Low, "Jack");
            Assert.That(queue.Count, Is.EqualTo(1));

            string name = queue.Dequeue();
            Assert.That(name, Is.EqualTo("Jack"));
            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddTwo_DifferentPriority()
        {
            var queue = new PriorityQueue<Priority, string>();

            queue.Enqueue(Priority.Low, "Jack");
            queue.Enqueue(Priority.Medium, "Ben");
            Assert.That(queue.Count, Is.EqualTo(2));

            string name = queue.Dequeue();
            Assert.That(name, Is.EqualTo("Ben"));
            Assert.That(queue.Count, Is.EqualTo(1));

            name = queue.Dequeue();
            Assert.That(name, Is.EqualTo("Jack"));
            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddTwo_SamePriority()
        {
            var queue = new PriorityQueue<Priority, string>();

            queue.Enqueue(Priority.Low, "Jack");
            queue.Enqueue(Priority.Low, "Kate");
            Assert.That(queue.Count, Is.EqualTo(2));

            string name = queue.Dequeue();
            Assert.That(name, Is.EqualTo("Jack"));
            Assert.That(queue.Count, Is.EqualTo(1));

            name = queue.Dequeue();
            Assert.That(name, Is.EqualTo("Kate"));
            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void AddMultiple_SamePriority()
        {
            var queue = new PriorityQueue<Priority, int>();

            for(int i = 0; i < 10; i++)
            {
                queue.Enqueue(Priority.Medium, i);
            }

            queue.Enqueue(Priority.Low, -1);

            for(int i = 0; i < 10; i++)
            {
                int value = queue.Dequeue();
                Assert.That(value >= 0 && value <= 9);
            }

            int x = queue.Dequeue();
            Assert.That(x, Is.EqualTo(-1));
        }

        [Test]
        public void Add()
        {
            var queue = new PriorityQueue<Priority, string>();

            queue.Enqueue(Priority.Medium, "X");
            queue.Enqueue(Priority.High, "Y");
            queue.Enqueue(Priority.Low, "Z");

            Assert.That(queue.Count, Is.EqualTo(3));

            Assert.That(queue.Dequeue(), Is.EqualTo("Y"));
            Assert.That(queue.Dequeue(), Is.EqualTo("X"));
            Assert.That(queue.Dequeue(), Is.EqualTo("Z"));
        }

        [Test]
        public void TryDequeue()
        {
            var queue = new PriorityQueue<Priority, string>();

            Priority p;
            string v;

            Assert.IsFalse(queue.TryDequeue(out p, out v));

            queue.Enqueue(Priority.Low, "Sawyer");
            Assert.IsTrue(queue.TryDequeue(out p, out v));
            Assert.That(p, Is.EqualTo(Priority.Low));
            Assert.That(v, Is.EqualTo("Sawyer"));

            Assert.That(queue.Count, Is.EqualTo(0));
        }

        [Test]
        public void Peek()
        {
            var queue = CreateStringQueue();

            queue.Enqueue(Priority.Low, "Jack");
            Assert.That(queue.Peek(), Is.EqualTo("Jack"));
            Assert.That(queue.Peek(), Is.EqualTo("Jack"));

            Assert.That(queue.Count, Is.EqualTo(1));
        }

        [Test]
        public void Peek_Fail()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var queue = CreateStringQueue();
                var name = queue.Peek();
            });
        }

        protected abstract IPriorityQueue<Priority, string> CreateStringQueue();

        protected abstract IPriorityQueue<Priority, int> CreateIntegerQueue();
    }

    public enum Priority
    {
        Low, Medium, High
    }
}
