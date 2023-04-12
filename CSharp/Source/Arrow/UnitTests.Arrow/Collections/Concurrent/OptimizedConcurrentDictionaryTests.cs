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
    public class OptimizedConcurrentDictionaryTests : DictionaryTestBase
    {
        [Test]
        public void Initializers()
        {
            Assert.DoesNotThrow(() => new OptimizedConcurrentDictionary<string, int>());
            Assert.DoesNotThrow(() => new OptimizedConcurrentDictionary<string, int>(null));
            Assert.DoesNotThrow(() => new OptimizedConcurrentDictionary<string, int>(StringComparer.OrdinalIgnoreCase));

            Assert.DoesNotThrow(() => new OptimizedConcurrentDictionary<string, int>(1, 8));
            Assert.DoesNotThrow(() => new OptimizedConcurrentDictionary<string, int>(1, 8, null));
            Assert.DoesNotThrow(() => new OptimizedConcurrentDictionary<string, int>(1, 8, StringComparer.OrdinalIgnoreCase));
        }

        [Test]
        public void IndexAccess()
        {
            var dictionary = new OptimizedConcurrentDictionary<string, int>();
            dictionary["Jack"] = 42;
            Assert.That(dictionary["Jack"], Is.EqualTo(42));

            IOptimizedConcurrentDictionary<string, int> asInterface = dictionary;
            asInterface["Ben"] = 20;
            Assert.That(asInterface["Jack"], Is.EqualTo(42));
            Assert.That(asInterface["Ben"], Is.EqualTo(20));

            IReadOnlyOptimizedConcurrentDictionary<string, int> asReadOnly = asInterface;
            Assert.That(asReadOnly["Jack"], Is.EqualTo(42));
            Assert.That(asReadOnly["Ben"], Is.EqualTo(20));
        }

        protected override IOptimizedConcurrentDictionary<string, int> MakeDictionary(IEqualityComparer<string> comparer)
        {
            return new OptimizedConcurrentDictionary<string, int>(comparer);
        }
    }
}
