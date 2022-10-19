using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Threading;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading
{
    [TestFixture]
    public class CombineCancellationTokenTests
    {
        [Test]
        public void BothAreDefault()
        {
            using (var d = CombineCancellationToken.Make(default, default, out var combined))
            {
                Assert.That(d, Is.Null);
                Assert.That(combined, Is.EqualTo(CancellationToken.None));
            }
        }

        [Test]
        public void FirstIsDefault()
        {
            using(var source = new CancellationTokenSource())
            {
                var token = source.Token;

                using (var d = CombineCancellationToken.Make(token, default, out var combined))
                {
                    Assert.That(d, Is.Null);
                    Assert.That(combined, Is.EqualTo(token));
                }
            }
        }

        [Test]
        public void SecondIsDefault()
        {
            using(var source = new CancellationTokenSource())
            {
                var token = source.Token;

                using (var d = CombineCancellationToken.Make(default, token, out var combined))
                {
                    Assert.That(d, Is.Null);
                    Assert.That(combined, Is.EqualTo(token));
                }
            }
        }

        [Test]
        public void BothAreNotDefault()
        {
            using(var source1 = new CancellationTokenSource())
            using(var source2 = new CancellationTokenSource())
            {
                var token1 = source1.Token;
                var token2 = source2.Token;

                using (var d = CombineCancellationToken.Make(token1, token2, out var combined))
                {
                    Assert.That(d, Is.Not.Null);
                    Assert.That(combined, Is.Not.EqualTo(CancellationToken.None));
                }
            }
        }
    }
}
