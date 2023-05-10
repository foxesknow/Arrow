using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.InsideOut.Transport;
using Arrow.InsideOut.Transport.Http.Client;

using NUnit.Framework;

namespace UnitTests.Arrow.InsideOut.Transport.Http.Client
{
    [TestFixture]
    public class HttpClientManagerTests
    {
        private static readonly PublisherID Publisher = new(Environment.MachineName, "Jack");

        [Test]
        public void Register()
        {
            using (var manager = new HttpClientManager())
            {
                var node = manager.Register(Publisher, new("http://localhost:8080/InsideOut"));
                Assert.That(node, Is.Not.Null);
                Assert.That(manager.IsRegistered(Publisher), Is.True);
            }
        }

        [Test]
        public void RegisterTwice()
        {
            using (var manager = new HttpClientManager())
            {
                var node = manager.Register(Publisher, new("http://localhost:8080/InsideOut"));
                Assert.Catch(() => manager.Register(Publisher, new("http://localhost:8080/InsideOut")));
                Assert.Catch(() => manager.Register(Publisher, new("http://localhost:8088/InsideOut")));
            }
        }

        [Test]
        public void TryGetNode()
        {
            using (var manager = new HttpClientManager())
            {
                manager.Register(Publisher, new("http://localhost:8080/InsideOut"));
                Assert.That(manager.TryGetNode(Publisher, out var node), Is.True);
                Assert.That(node, Is.Not.Null);
            }
        }

        [Test]
        public void TryGetNode_NotFound()
        {
            using (var manager = new HttpClientManager())
            {
                Assert.That(manager.TryGetNode(Publisher, out var node), Is.False);
                Assert.That(node, Is.Null);
            }
        }

        [Test]
        public void Disposed()
        {
            var manager = new HttpClientManager();
            manager.Dispose();

            Assert.Catch(() => manager.Register(Publisher, new("http://localhost:8080/InsideOut")));
        }
    }
}
