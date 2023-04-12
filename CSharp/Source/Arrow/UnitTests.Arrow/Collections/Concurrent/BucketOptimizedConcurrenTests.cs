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
    public class BucketOptimizedConcurrenTests : DictionaryTestBase
    {
        [Test]
        public void InvalidNumberOfBuckets()
        {
            Assert.Catch(() =>  BucketOptimizedConcurrentDictionary.Make<string, int>(0));
            Assert.Catch(() =>  BucketOptimizedConcurrentDictionary.Make<string, int>(-1));
        }

        [Test]
        public void SingleBucket()
        {
            var dictionary = BucketOptimizedConcurrentDictionary.Make<string, int>(1);
            Assert.That(dictionary, Is.InstanceOf<OptimizedConcurrentDictionary<string, int>>());
        }

        [Test]
        public void FillWithData()
        {
            const int reps = 1000;
            var dictionary = BucketOptimizedConcurrentDictionary.Make<string, int>(7);

            Assert.That(dictionary.HasData(), Is.False);
            Assert.That(dictionary.IsEmpty(), Is.True);

            for(var i = 0; i < reps; i++)
            {
                var key = $"Jack-{i}";
                Assert.That(dictionary.TryAdd(key, i), Is.True);
                Assert.That(dictionary.TryAdd(key, i), Is.False);
                Assert.That(dictionary[key], Is.EqualTo(i));

                Assert.That(dictionary.TryGetValue(key, out var value), Is.True);
                Assert.That(value, Is.EqualTo(i));

                var updatedValue = dictionary.AddOrUpdate(key, -1, (key, existing) => existing * 2);
                Assert.That(dictionary.TryGetValue(key, out value), Is.True);
                Assert.That(value, Is.EqualTo(updatedValue));
            }

            Assert.That(dictionary.Count(), Is.EqualTo(reps));
            Assert.That(dictionary.Keys().Count(), Is.EqualTo(reps));
            Assert.That(dictionary.Values().Count(), Is.EqualTo(reps));

            dictionary.Clear();

            Assert.That(dictionary.Count(), Is.EqualTo(0));
            Assert.That(dictionary.Keys().Count(), Is.EqualTo(0));
            Assert.That(dictionary.Values().Count(), Is.EqualTo(0));

            Assert.That(dictionary.HasData(), Is.False);
            Assert.That(dictionary.IsEmpty(), Is.True);
        }

        protected override IOptimizedConcurrentDictionary<string, int> MakeDictionary(IEqualityComparer<string> comparer)
        {
            return BucketOptimizedConcurrentDictionary.Make<string, int>(7, comparer);
        }
    }
}
