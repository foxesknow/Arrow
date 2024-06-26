﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Arrow.Storage;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Storage
{
    [TestFixture]
    public class ResourceAccessorTests
    {
        [Test]
        public void Load()
        {
            Uri uri = new Uri("res://UnitTests.Arrow/UnitTests/Arrow/Resources/Jack.xml");
            var access = new ResourceAccessor(uri);

            using(Stream stream = access.OpenRead())
            {
                Assert.IsNotNull(stream);

                using(StreamReader reader = new StreamReader(stream))
                {
                    string data = reader.ReadToEnd();
                    Assert.That(data, Is.Not.Empty);
                }
            }
        }

        [Test]
        public void CreateResourceUri()
        {
            Uri uri1 = ResourceAccessor.CreateUri(typeof(ResourceAccessorTests).Assembly, "path/to/resource.xml");
            Assert.That(uri1, Is.Not.Null);

            Uri uri2 = ResourceAccessor.CreateUri(typeof(ResourceAccessorTests).Assembly, "/path/to/resource.xml");
            Assert.That(uri2, Is.Not.Null);

            Assert.That(uri1, Is.EqualTo(uri2));

            var builder = new UriBuilder(uri1);
            Assert.That(builder.Scheme, Is.EqualTo(ResourceAccessor.Scheme));
        }

        [Test]
        public void TestNotFound()
        {
            Assert.Throws<IOException>(() =>
            {
                Uri uri = new Uri("res://UnitTests.Arrow/this/does/not/exist.xml");
                var access = new ResourceAccessor(uri);

                using(Stream stream = access.OpenRead())
                {
                    // We should never get here
                    Assert.Fail();
                }
            });
        }

        [Test]
        public void Exists()
        {
            Uri uri = ResourceAccessor.CreateUri(typeof(ResourceAccessorTests).Assembly, "UnitTests/Arrow/Resources/Jack.xml");
            var access = new ResourceAccessor(uri);
            Assert.IsTrue(access.CanExists);
            Assert.IsTrue(access.Exists());
        }

        [Test]
        public void NotExists()
        {
            Uri uri = new Uri("res://UnitTests.Arrow/this/does/not/exist.xml");
            var access = new ResourceAccessor(uri);
            Assert.IsTrue(access.CanExists);
            Assert.IsFalse(access.Exists());
        }
    }
}
