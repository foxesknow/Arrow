using System;
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
    public class StorageManagerTests
    {
        [Test]
        public void TryGetAcccessor()
        {
            Accessor accessor = null;

            Assert.IsTrue(StorageManager.TryGetAcccessor(new Uri("http://foo/bar.txt"), out accessor));
            Assert.IsTrue(StorageManager.TryGetAcccessor(new Uri("HTTP://foo/bar.txt"), out accessor));
            Assert.IsFalse(StorageManager.TryGetAcccessor(new Uri("dharma://foo/bar.txt"), out accessor));
        }

        [Test]
        public void Contains()
        {
            // Names are case-insensitive
            Assert.IsTrue(StorageManager.Contains("http"));
            Assert.IsTrue(StorageManager.Contains("HTTP"));

            Assert.IsTrue(StorageManager.Contains("file"));
            Assert.IsFalse(StorageManager.Contains(""));
        }

        [Test]
        public void Get()
        {
            Assert.That(StorageManager.Get(new Uri("http://foo/bar.txt")), Is.Not.Null);
            Assert.That(StorageManager.Get(new Uri("HTTP://foo/bar.txt")), Is.Not.Null);
        }

        [Test]
        public void Get_WillFail()
        {
            Assert.Throws<IOException>(() =>
            {
                StorageManager.Get(new Uri("foo://bar/file.txt"));
            });
        }
    }
}
