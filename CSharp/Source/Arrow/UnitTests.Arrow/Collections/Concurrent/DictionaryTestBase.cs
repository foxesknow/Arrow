using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections.Concurrent;

using NUnit.Framework;

namespace UnitTests.Arrow.Collections.Concurrent
{
    public abstract class DictionaryTestBase
    {
        [Test]
        public void Initialization()
        {
            var dictionary = MakeDictionary();
            Assert.That(dictionary.HasData(), Is.False);
            Assert.That(dictionary.IsEmpty(), Is.True);
        }

        [Test]
        public void Initialization_IgnoreCase()
        {
            var dictionary = MakeDictionary(StringComparer.OrdinalIgnoreCase);
            Assert.That(dictionary.HasData(), Is.False);
            Assert.That(dictionary.IsEmpty(), Is.True);

            dictionary["Jack"] = 42;
            Assert.That(dictionary["jack"], Is.EqualTo(42));

            dictionary.AddOrUpdate("JACK", 50, (key, existing) => existing + 10);
            Assert.That(dictionary["Jack"], Is.EqualTo(52));
            Assert.That(dictionary["jack"], Is.EqualTo(52));
            Assert.That(dictionary["JACK"], Is.EqualTo(52));
        }

        [Test]
        public void AddWhilstIterating()
        {
            var dictionary = MakeDictionary();
            dictionary.TryAdd("Ben", 1);
            dictionary.TryAdd("Jack", 3);
            dictionary.TryAdd("Sawyer", 5);
            dictionary.TryAdd("Kate", 7);

            foreach(var pair in dictionary)
            {
                if(pair.Value < 10)
                {
                    var newKey = $"{pair.Key}-new";
                    Assert.That(dictionary.TryAdd(newKey, pair.Value * 1000), Is.True);
                }
            }

            Assert.That(dictionary.Count(), Is.EqualTo(8));
        }

        [Test]
        public void AddOrUpdate_AddFactory()
        {
            var dictionary = MakeDictionary();
            
            var first = dictionary.AddOrUpdate("Jack", key => 10, (key, existing) => existing + 5);
            Assert.That(first, Is.EqualTo(10));
            Assert.That(dictionary["Jack"], Is.EqualTo(10));

            var second = dictionary.AddOrUpdate("Jack", key => 10, (key, existing) => existing + 5);
            Assert.That(second, Is.EqualTo(15));
            Assert.That(dictionary["Jack"], Is.EqualTo(15));
        }

        [Test]
        public void AddOrUpdate()
        {
            var dictionary = MakeDictionary();
            
            var first = dictionary.AddOrUpdate("Jack", 10, (key, existing) => existing + 5);
            Assert.That(first, Is.EqualTo(10));
            Assert.That(dictionary["Jack"], Is.EqualTo(10));

            var second = dictionary.AddOrUpdate("Jack", 10, (key, existing) => existing + 5);
            Assert.That(second, Is.EqualTo(15));
            Assert.That(dictionary["Jack"], Is.EqualTo(15));
        }

        [Test]
        public void AddOrReplace()
        {
            var dictionary = MakeDictionary();
            
            dictionary.AddOrReplace("Jack", 10);
            Assert.That(dictionary["Jack"], Is.EqualTo(10));

            dictionary.AddOrReplace("Jack", 15);
            Assert.That(dictionary["Jack"], Is.EqualTo(15));
        }

        [Test]
        public void Clear()
        {
            var dictionary = MakeDictionary();
            
            dictionary.AddOrReplace("Jack", 10);
            Assert.That(dictionary.ContainsKey("Jack"), Is.True);

            dictionary.Clear();
            Assert.That(dictionary.ContainsKey("Jack"), Is.False);
        }

        [Test]
        public void ContainsKey()
        {
            var dictionary = MakeDictionary();
            
            dictionary.AddOrReplace("Jack", 10);
            Assert.That(dictionary.ContainsKey("Jack"), Is.True);
            Assert.That(dictionary.HasData(), Is.True);

            dictionary.Clear();
            Assert.That(dictionary.ContainsKey("Jack"), Is.False);
            Assert.That(dictionary.HasData(), Is.False);
        }

        [Test]
        public void GetOrAdd_Function()
        {
            var dictionary = MakeDictionary();
            
            var first = dictionary.GetOrAdd("Jack", key => 10);
            Assert.That(first, Is.EqualTo(10));
            Assert.That(dictionary["Jack"], Is.EqualTo(10));

            var second = dictionary.GetOrAdd("Jack", key => 20);
            Assert.That(second, Is.EqualTo(10));
            Assert.That(dictionary["Jack"], Is.EqualTo(10));
        }

        [Test]
        public void GetOrAdd()
        {
            var dictionary = MakeDictionary();
            
            var first = dictionary.GetOrAdd("Jack", 10);
            Assert.That(first, Is.EqualTo(10));
            Assert.That(dictionary["Jack"], Is.EqualTo(10));

            var second = dictionary.GetOrAdd("Jack", 20);
            Assert.That(second, Is.EqualTo(10));
            Assert.That(dictionary["Jack"], Is.EqualTo(10));
        }

        [Test]
        public void TryAdd()
        {
            var dictionary = MakeDictionary();
            Assert.That(dictionary.TryAdd("Jack", 10), Is.True);
            Assert.That(dictionary.TryAdd("Jack", 20), Is.False);
            
            Assert.That(dictionary["Jack"], Is.EqualTo(10));
        }

        [Test]
        public void TryGetValue()
        {
            var dictionary = MakeDictionary();
            Assert.That(dictionary.TryGetValue("Jack", out var value), Is.False);
            Assert.That(value, Is.EqualTo(0));
            
            Assert.That(dictionary.TryAdd("Jack", 10), Is.True);

            Assert.That(dictionary.TryGetValue("Jack", out value), Is.True);
            Assert.That(value, Is.EqualTo(10));
        }

        [Test]
        public void TryRemove()
        {
            var dictionary = MakeDictionary();
            Assert.That(dictionary.TryRemove("Jack", out var value), Is.False);
            Assert.That(value, Is.EqualTo(0));
            
            dictionary["Jack"] = 10;

            Assert.That(dictionary.TryRemove("Jack", out value), Is.True);
            Assert.That(value, Is.EqualTo(10));
        }

        [Test]
        public void TryUpdate()
        {
            var dictionary = MakeDictionary();
            dictionary["Jack"] = 20;

            Assert.That(dictionary.TryUpdate("Jack", 42, 10), Is.False);
            Assert.That(dictionary["Jack"], Is.EqualTo(20));
            
            Assert.That(dictionary.TryUpdate("Jack", 42, 20), Is.True);
            Assert.That(dictionary["Jack"], Is.EqualTo(42));
        }

        [Test]
        public void Keys()
        {
            var dictionary = MakeDictionary();
            Assert.That(dictionary.Keys().Contains("Jack"), Is.False);

            dictionary["Jack"] = 20;
            Assert.That(dictionary.Keys().Contains("Jack"), Is.True);
        }

        [Test]
        public void Values()
        {
            var dictionary = MakeDictionary();
            Assert.That(dictionary.Values().Contains(20), Is.False);

            dictionary["Jack"] = 20;
            Assert.That(dictionary.Values().Contains(20), Is.True);
        }

        [Test]
        public void Enumerator()
        {
            var dictionary = MakeDictionary();
            Assert.That(dictionary.Count(), Is.EqualTo(0));

            dictionary["Jack"] = 20;
            Assert.That(dictionary.Count(), Is.EqualTo(1));
        }

        [Test]
        public void SequencesAreNotCollections()
        {
            var dictionary = MakeDictionary();
            Assert.That(dictionary.Keys(), Is.Not.InstanceOf<ICollection<string>>());
            Assert.That(dictionary.Keys(), Is.Not.InstanceOf<System.Collections.ICollection>());

            Assert.That(dictionary.Values(), Is.Not.InstanceOf<ICollection<int>>());
            Assert.That(dictionary.Values(), Is.Not.InstanceOf<System.Collections.ICollection>());
        }

        protected IOptimizedConcurrentDictionary<string, int> MakeDictionary()
        {
            return MakeDictionary(null);
        }

        protected abstract IOptimizedConcurrentDictionary<string, int> MakeDictionary(IEqualityComparer<string> comparer);
    }
}
