using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections.Concurrent;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections.Concurrent
{
    [TestFixture]
    public class BucketConcurrentHashSetTests : HashSetTestBase
    {
        [Test]
        public void Initializers()
        {
            Assert.DoesNotThrow(() => BucketConcurrentHashSet.Make<string>(7));
            Assert.DoesNotThrow(() => BucketConcurrentHashSet.Make<string>(7, null));
            Assert.DoesNotThrow(() => BucketConcurrentHashSet.Make<string>(7, StringComparer.OrdinalIgnoreCase));
        }

        [Test]
        public void InvalidNumberOfBuckets()
        {
            Assert.Catch(() => BucketConcurrentHashSet.Make<string>(0));
            Assert.Catch(() => BucketConcurrentHashSet.Make<string>(-1));
        }

        [Test]
        public void SingleBucket()
        {
            var set = BucketConcurrentHashSet.Make<string>(1);
            Assert.That(set, Is.InstanceOf<ConcurrentHashSet<string>>());
        }

        [Test]
        public void FillWithData()
        {
            const int reps = 1000;
            var set = BucketConcurrentHashSet.Make<string>(7);

            Assert.That(set.HasData(), Is.False);
            Assert.That(set.IsEmpty(), Is.True);

            for(var i = 0; i < reps; i++)
            {
                var key = $"Jack-{i}";
                Assert.That(set.Add(key), Is.True);
                Assert.That(set.Add(key), Is.False);
                Assert.That(set.Contains(key), Is.True);
            }

            Assert.That(set.HasData(), Is.True);
            Assert.That(set.IsEmpty(), Is.False);

            Assert.That(set.Count(), Is.EqualTo(reps));

            set.Clear();

            Assert.That(set.HasData(), Is.False);
            Assert.That(set.IsEmpty(), Is.True);

            Assert.That(set.Count(), Is.EqualTo(0));
        }

        protected override IConcurrentHashSet<T> Make<T>(IEqualityComparer<T> comparer)
        {
            return BucketConcurrentHashSet.Make(7, comparer);
        }
    }
}
