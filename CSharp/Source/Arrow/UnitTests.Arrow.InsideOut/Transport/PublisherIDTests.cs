using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut.Transport;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut.Transport
{
    [TestFixture]
    public class PublisherIDTests
    {
        [Test]
        public void Initialization_BadArguments()
        {
            Assert.Catch(() => new PublisherID(null, null));
            Assert.Catch(() => new PublisherID("foo", null));
            Assert.Catch(() => new PublisherID(null, "bar"));
            
            Assert.Catch(() => new PublisherID("", "bar"));
            Assert.Catch(() => new PublisherID("foo", ""));
            Assert.Catch(() => new PublisherID("", ""));
        }

        [Test]
        public void Initialization()
        {
            var p = new PublisherID("Server", "Instance");
            Assert.That(p.ServerName, Is.EqualTo("Server"));
            Assert.That(p.InstanceName, Is.EqualTo("Instance"));

            Assert.That(p.ToString(), Is.Not.Null & Has.Length.GreaterThan(0));
        }

        [Test]
        public void Equality_SameInstance()
        {
            var p = new PublisherID("Server", "Instance");
            Assert.That(p, Is.EqualTo(p));
            Assert.That(p.Equals(p), Is.True);
        }

        [Test]
        public void Equality_DifferentCase()
        {
            var p1 = new PublisherID("Server", "Instance");
            var p2 = new PublisherID("SERVER", "INSTANCE");

            Assert.That(p1, Is.EqualTo(p2));
            Assert.That(p1.GetHashCode(), Is.EqualTo(p2.GetHashCode()));
        }

        [Test]
        public void Equality_Different()
        {
            var p1 = new PublisherID("Server", "Instance");
            var p2 = new PublisherID("foo", "bar");

            Assert.That(p1.GetHashCode(), Is.Not.EqualTo(p2.GetHashCode()));
        }

        [Test]
        public void EncodeDecode()
        {
            var p1 = new PublisherID("Server", "Instance");
            var asString = p1.Encode();
            Assert.That(asString, Is.Not.Null & Has.Length.GreaterThan(0));

            var p2 = PublisherID.Decode(asString);
            Assert.That(p2.ServerName, Is.EqualTo("Server"));
            Assert.That(p2.InstanceName, Is.EqualTo("Instance"));
            Assert.That(p1, Is.EqualTo(p2));
        }
    }
}
