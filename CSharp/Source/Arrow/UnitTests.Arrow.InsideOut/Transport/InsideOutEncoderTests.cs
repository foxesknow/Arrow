using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut.Transport;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut.Transport
{
    [TestFixture]
    public class InsideOutEncoderTests
    {
        [Test]
        public void HasDefault()
        {
            Assert.That(InsideOutEncoder.Default, Is.Not.Null);
        }

        [Test]
        public void EncodeToPool()
        {
            var encoder = InsideOutEncoder.Default;
            using(var returner = encoder.EncodeToPool("Hello Jack"))
            {
                Assert.That(returner.AsSpan().Length, Is.Not.Zero);

                var buffer = returner.AsSpan().ToArray();
                var roundTrip = encoder.Decode<string>(buffer, 0, buffer.Length);
                Assert.That(roundTrip, Is.EqualTo("Hello Jack"));
            }
        }

        [Test]
        public void EncodeToMemory()
        {
            var encoder = InsideOutEncoder.Default;
            var memory = encoder.EncodeToMemory("Hello Jack");
            Assert.That(memory.Length, Is.Not.Zero);

            var buffer = memory.ToArray();
            var roundTrip = encoder.Decode<string>(buffer, 0, buffer.Length);
            Assert.That(roundTrip, Is.EqualTo("Hello Jack"));
        }

        [Test]
        public void Encode()
        {
            var encoder = InsideOutEncoder.Default;

            using(var stream = new MemoryStream())
            {
                encoder.Encode("Hello Jack", stream);
                stream.Position = 0;
                
                var roundTrip = encoder.Decode<string>(stream);
                Assert.That(roundTrip, Is.EqualTo("Hello Jack"));
            }
        }
    }
}
