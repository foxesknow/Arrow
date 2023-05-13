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
    public class MultiDictionaryTests
    {
        [Test]
        public void Add()
        {
            MultiDictionary<string, int> data = new MultiDictionary<string, int>();

            data.Add("Bob", 1);
            data.Add("Bob", 2, 4, 8);

            Assert.IsTrue(data.KeyCount == 1);
            Assert.IsTrue(data.Count == 4);

            Assert.IsTrue(data.ContainsValue(4));
            Assert.IsFalse(data.ContainsValue(3));

            IList<int> numbers = new int[] { 3, 5, 7, 9 };
            data.Add("Jack", numbers);

            Assert.IsTrue(data.KeyCount == 2);
            Assert.IsTrue(data.Count == 8);
        }

        [Test]
        public void Clear()
        {
            MultiDictionary<string, int> data = new MultiDictionary<string, int>();

            data.Add("Locke", 1, 2, 3);
            data.Add("Sawyer", 8, 16);

            // Remove values for a key
            Assert.IsTrue(data.ValuesFor("Sawyer").Count == 2);
            data.ClearValues("Sawyer");
            Assert.IsTrue(data.ValuesFor("Sawyer").Count == 0);
            Assert.IsTrue(data.ContainsKey("Sawyer"));

            data.Clear();
            Assert.IsTrue(data.KeyCount == 0);
            Assert.IsTrue(data.Count == 0);
        }

        [Test]
        public void Iteration()
        {
            MultiDictionary<string, int> data = new MultiDictionary<string, int>();

            data.Add("Locke", 1, 2, 3);
            data.Add("Sawyer", 8, 16);

            int lockeCount = 0;
            foreach(int i in data.ValuesFor("Locke"))
            {
                lockeCount += i;
            }

            Assert.IsTrue(lockeCount == 6);

            int countAll = 0;
            foreach(KeyValuePair<string, int> pair in data)
            {
                countAll += pair.Value;
            }

            Assert.IsTrue(countAll == 30);

            foreach(KeyValuePair<string, IList<int>> pair in data.Sequences)
            {
                if(pair.Key == "Locke") Assert.IsTrue(pair.Value.Count == 3);
                else if(pair.Key == "Sawyer") Assert.IsTrue(pair.Value.Count == 2);
                else Assert.Fail();
            }
        }

        [Test]
        public void Remove()
        {
            MultiDictionary<string, int> data = new MultiDictionary<string, int>();

            data.Add("Locke", 1, 2, 3);
            data.Add("Sawyer", 8, 16);
            data.Add("Eko", 42, 58);

            Assert.IsTrue(data.KeyCount == 3);

            data.Remove("Locke");
            Assert.IsTrue(data.KeyCount == 2);
            Assert.IsFalse(data.ContainsKey("Locke"));

            IList<int> numbers = null;
            Assert.IsFalse(data.TryGetValue("Locke", out numbers));
        }

        [Test]
        public void Lookup()
        {
            MultiDictionary<string, int> data = new MultiDictionary<string, int>();
            data.Add("Locke", 1, 2, 3);
            data.Add("Sawyer", 8, 16);
            data.Add("Eko", 42, 58);

            IList<int> numbers = null;
            Assert.IsFalse(data.TryGetValue("Jack", out numbers));
            Assert.IsNull(numbers);

            Assert.IsTrue(data.TryGetValue("Sawyer", out numbers));
            Assert.IsNotNull(numbers);
            Assert.IsTrue(numbers.Count == 2);

            numbers = data.ValuesFor("Locke");
            Assert.IsNotNull(numbers);
            Assert.IsTrue(numbers.Count == 3);
        }
    }
}
