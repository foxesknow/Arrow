using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Net;

using NUnit.Framework;


namespace UnitTests.Arrow.Net
{
    [TestFixture]
    public class UriManagerTests
    {
        [Test]
        public void Initialization()
        {
            Assert.DoesNotThrow(() => new UriManager());
        }

        [Test]
        public void FromAppConfig_ReturnsSameInstance()
        {
            var m1 = UriManager.FromAppConfig();
            var m2 = UriManager.FromAppConfig();

            Assert.That(m1, Is.SameAs(m2));
        }

        [Test]
        public void Register()
        {
            var manager = new UriManager();
            Assert.That(manager.TryGetUri("foo", out var _), Is.False);

            manager.Register("foo", new Uri("http://www.island.com/"));
            Assert.That(manager.TryGetUri("foo", out var uri), Is.True);
            Assert.That(uri.ToString(), Is.EqualTo("http://www.island.com/"));
        }

        [Test]
        public void Register_ReplaceExisting()
        {
            var manager = new UriManager();
            Assert.That(manager.TryGetUri("foo", out var _), Is.False);

            manager.Register("foo", new Uri("http://www.island.com/"));
            manager.Register("foo", new Uri("http://www.tempest.com/"));

            Assert.That(manager.TryGetUri("foo", out var uri), Is.True);
            Assert.That(uri.ToString(), Is.EqualTo("http://www.tempest.com/"));
        }

        [Test]
        public void FromAppConfig()
        {
            var manager = UriManager.FromAppConfig();

            Assert.That(manager.TryGetUri("userlookup", out var userLookupUri), Is.True);
            Assert.That(userLookupUri, Is.Not.Null);

            var userLookupUri2 = manager.GetUri("UserLookup");
            Assert.That(userLookupUri2, Is.Not.Null);
            Assert.That(userLookupUri, Is.SameAs(userLookupUri2));
        }

        [Test]
        public void ParametersFromTemplate()
        {
            var manager = UriManager.FromAppConfig();

            Assert.That(manager.TryGetUri("LookupJack", out var lookupJack), Is.True);
            Assert.That(lookupJack, Is.Not.Null);

            var parameters = lookupJack.QueryParameters().ToList();
            Assert.That(parameters.Count, Is.EqualTo(2));

            Assert.That(parameters[0].Name, Is.EqualTo("user"));
            Assert.That(parameters[0].Value, Is.EqualTo("Jack"));

            Assert.That(parameters[1].Name, Is.EqualTo("age"));
            Assert.That(parameters[1].Value, Is.EqualTo("39"));
        }

        [Test]
        public void EverythingSet_1()
        {
            var manager = UriManager.FromAppConfig();

            Assert.That(manager.TryGetUri("EverythingSet-1", out var everything), Is.True);
            Assert.That(everything, Is.Not.Null);

            Assert.That(everything.Scheme, Is.EqualTo("https"));
            Assert.That(everything.Port, Is.EqualTo(8080));
            Assert.That(everything.Host, Is.EqualTo("www.foo.com"));
            Assert.That(everything.UserInfo, Is.EqualTo("Bob:123"));
            Assert.That(everything.PathAndQuery, Is.EqualTo("/some/where"));
        }
    }
}
