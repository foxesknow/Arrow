using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

using Arrow.IO;

using NUnit.Framework;

namespace UnitTests.Arrow.IO
{
    [TestFixture]
    public class DirectoryExpanderTests
    {
        [Test]
        public void Expand()
        {
            var directory = Path.Combine(Environment.SystemDirectory, DirectoryExpander.ExpansionPoint);
            var directories = DirectoryExpander.Expand(directory).ToList();
            Assert.That(directories.Count, Is.GreaterThan(0));
        }

        [Test]
        public void NoSuchDirectory()
        {
            var directory = Path.Combine(Environment.SystemDirectory, "no-such-directory-wierjwoeirjweoi", DirectoryExpander.ExpansionPoint);
            var directories = DirectoryExpander.Expand(directory).ToList();
            Assert.That(directories.Count, Is.EqualTo(0));
        }

        [Test]
        public void Everything()
        {
            var directory = Path.Combine(Environment.SystemDirectory, "no-such-directory-wierjwoeirjweoi");
            var directories = DirectoryExpander.Expand(directory, DirectoryExpanderMode.Everything).ToList();
            Assert.That(directories.Count, Is.EqualTo(1));
        }
    }
}
