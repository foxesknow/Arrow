using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections;
using Arrow.Net;
using NUnit.Framework;

namespace UnitTests.Arrow.Net
{
    [TestFixture]
    public class UriExtensionsTests
    {
        [Test]
        public void AddParameter()
        {
            var uri = new Uri("http://www.island.com?name=Jack");
            var added = uri.AddParameter("age", "10");

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Query, Contains.Substring("age=10"));
        }

        [Test]
        public void AddOrParameter()
        {
            var uri = new Uri("http://www.island.com?name=Jack");
            var added = uri.AddParameter("name", "Ben");

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Query, Contains.Substring("name=Ben"));
        }

        [Test]
        public void RemoveParameter()
        {
            var uri = new Uri("http://www.island.com?name=Jack");
            var added = uri.RemoveParameter("name");

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Query, Does.Not.Contains("name=Jack"));
        }

        [Test]
        public void QueryParameters()
        {
            var uri = new Uri("http://www.island.com?name=Jack&age=39");
            var parameters = uri.QueryParameters();

            Assert.That(parameters, Is.Not.Null);
            Assert.That(parameters.Count(), Is.EqualTo(2));
        }

        [Test]
        public void ClearQuery()
        {
            var uri = new Uri("http://www.island.com?name=Jack");
            var added = uri.ClearQuery();

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Query, Does.Not.Contain("name"));
        }

        [Test]
        public void StripQuery()
        {
            var uri = new Uri("http://www.island.com?name=Jack");
            var added = uri.StripQuery();

            Assert.That(added, Is.Not.Null);
            Assert.That(added.Query, Does.Not.Contain("name"));
        }

        [Test]
        public void StripLogonDetails()
        {
            var uri = new Uri("http://ben:123@www.island.com?name=Jack");
            var added = uri.StripLogonDetails();

            Assert.That(added, Is.Not.Null);
            Assert.That(added.ToString(), Does.Not.Contain("ben"));
            Assert.That(added.ToString(), Does.Not.Contain("123"));
        }
    }
}
