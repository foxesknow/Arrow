using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Numeric;

using NUnit.Framework;

namespace UnitTests.Arrow.Numeric
{   
    [TestFixture]
    public class PrimeSequenceTests
    {
        [Test]
        public void First1000()
        {
            var primes = PrimeSequence.First1000;
            Assert.That(primes, Is.Not.Null);
            Assert.That(primes.Count(), Is.EqualTo(1000));

            Assert.That(primes.Numbers[0], Is.EqualTo(2));

            // Everything else should be odd
            var last = 2;
            foreach(var number in primes.Skip(1))
            {
                Assert.That(number, Is.GreaterThan(last));
                Assert.That(number % 2, Is.EqualTo(1));

                last = number;
            }
        }

        [Test]
        public void TryGetFirstPrimeAfter()
        {
            var primes = PrimeSequence.First1000;
            Assert.That(primes.TryGetFirstPrimeAfter(0, out var value), Is.True);
            Assert.That(value, Is.EqualTo(2));

            Assert.That(primes.TryGetFirstPrimeAfter(7, out value), Is.True);
            Assert.That(value, Is.EqualTo(11));

            Assert.That(primes.TryGetFirstPrimeAfter(32, out value), Is.True);
            Assert.That(value, Is.EqualTo(37));
        }

        [Test]
        public void TryGetFirstPrimeAfter_NotAvailable()
        {
            var primes = PrimeSequence.First1000;
            var largest = primes.Last();

            Assert.That(primes.TryGetFirstPrimeAfter(largest, out var value), Is.False);
            Assert.That(value, Is.EqualTo(0));
        }

        [Test]
        public void FromArray()
        {
            var numbers = new[]{11, 7, 2, 3, 5};

            var primes = new PrimeSequence(numbers);
            Assert.That(primes.Count(), Is.EqualTo(5));

            Assert.That(primes.Numbers[0], Is.EqualTo(2));
            Assert.That(primes.Numbers[1], Is.EqualTo(3));
            Assert.That(primes.Numbers[2], Is.EqualTo(5));
            Assert.That(primes.Numbers[3], Is.EqualTo(7));
            Assert.That(primes.Numbers[4], Is.EqualTo(11));
        }
    }
}
