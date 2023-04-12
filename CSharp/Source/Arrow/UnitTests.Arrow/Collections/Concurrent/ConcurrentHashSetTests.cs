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
    public class ConcurrentHashSetTests : HashSetTestBase
    {
        [Test]
        public void Initializers()
        {
            Assert.DoesNotThrow(() => new ConcurrentHashSet<string>());
            Assert.DoesNotThrow(() => new ConcurrentHashSet<string>(null));
            Assert.DoesNotThrow(() => new ConcurrentHashSet<string>(8));
            Assert.DoesNotThrow(() => new ConcurrentHashSet<string>(8, StringComparer.OrdinalIgnoreCase));
        }

        protected override IConcurrentHashSet<T> Make<T>(IEqualityComparer<T> comparer)
        {
            return new ConcurrentHashSet<T>(comparer);
        }
    }
}
