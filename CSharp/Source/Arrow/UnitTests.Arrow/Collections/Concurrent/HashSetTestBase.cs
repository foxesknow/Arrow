using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections.Concurrent;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections.Concurrent
{
    public abstract class HashSetTestBase
    {
        [Test]
        public void Initialize()
        {
            var set = Make<int>();
            Assert.That(set.HasData(), Is.False);
            Assert.That(set.IsEmpty(), Is.True);
            Assert.That(set.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Duplicates()
        {
            var set = Make<int>();
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(1);

            Assert.That(set.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Duplicates_CaseInsensitive()
        {
            var set = Make<string>(StringComparer.OrdinalIgnoreCase);
            set.Add("A");
            set.Add("B");
            set.Add("C");
            set.Add("a");

            Assert.That(set.Count(), Is.EqualTo(3));
        }

        [Test]
        public void Add_CaseInsensitivity()
        {
            var set = Make<string>(StringComparer.OrdinalIgnoreCase);
            Assert.That(set.Add("Jack"), Is.True);
            Assert.That(set.Add("jack"), Is.False);
            Assert.That(set.Add("JACK"), Is.False);
        }

        [Test]
        public void Add_CaseSensitivity()
        {
            var set = Make<string>();
            Assert.That(set.Add("Jack"), Is.True);
            Assert.That(set.Add("jack"), Is.True);
            Assert.That(set.Add("JACK"), Is.True);
        }

        [Test]
        public void AddWhilstIterating()
        {
            var set = Make<int>();
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);

            // We should be able to mutate the collection whilst iterating
            foreach(var value in set)
            {
                set.Add(value * 1000);
            }

            Assert.That(set.Count(), Is.Not.EqualTo(4));
        }

        [Test]
        public void Remove()
        {
            var set = Make<int>();
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);

            Assert.That(set.Remove(2), Is.True);
            Assert.That(set.Count(), Is.EqualTo(3));
            Assert.That(set.Remove(2), Is.False);
        }

        [Test]
        public void Contains()
        {
            var set = Make<int>();
            set.Add(1);
            set.Add(2);
            set.Add(3);
            set.Add(4);

            Assert.That(set.Contains(2), Is.True);
            Assert.That(set.Remove(2), Is.True);
            Assert.That(set.Contains(2), Is.False);
        }

        protected IConcurrentHashSet<T> Make<T>()
        {
            return Make<T>(null);
        }

        protected abstract IConcurrentHashSet<T> Make<T>(IEqualityComparer<T> comparer);
    }
}
