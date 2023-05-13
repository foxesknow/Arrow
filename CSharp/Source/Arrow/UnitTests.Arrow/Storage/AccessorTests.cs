using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Storage;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Storage
{
    [TestFixture]
    public class AccessorTests
    {
        [Test]
        public void ResolveRelativeNoBase()
        {
            Uri uri = Accessor.ResolveRelative(null, "out.txt");
            Assert.IsNotNull(uri);
            Assert.That(uri.Scheme, Is.EqualTo(Uri.UriSchemeFile));

            uri = Accessor.ResolveRelative(null, "http://www.dharma.com");
            Assert.IsNotNull(uri);
            Assert.That(uri.Scheme, Is.EqualTo(Uri.UriSchemeHttp));
        }

        [Test]
        public void ResolveRelative()
        {
            Uri baseUri = new Uri("http://www.dharma.com/path/to/file.txt");

            Uri uri = Accessor.ResolveRelative(baseUri, "out.txt");
            Assert.IsNotNull(uri);
            Assert.That(uri, Is.EqualTo(new Uri("http://www.dharma.com/path/to/out.txt")));

            uri = Accessor.ResolveRelative(baseUri, "file://server/share/dir/out.txt");
            Assert.IsNotNull(uri);
            Assert.That(uri, Is.EqualTo(new Uri("file://server/share/dir/out.txt")));
        }
    }
}
