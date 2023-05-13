using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.IO;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.IO
{
    [TestFixture]
    public class SandboxDirectoryTests
    {
        [Test]
        public void Construction_Null()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                var sandbox = new SandboxDirectory(null);
            });
        }

        [Test]
        public void Construction()
        {
            var sandbox = new SandboxDirectory(@"c:\some\root");
        }

        [TestCase(@"", @"c:\some\root")]
        [TestCase(@"hello.txt", @"c:\some\root\hello.txt")]
        [TestCase(@"foo\hello.txt", @"c:\some\root\foo\hello.txt")]
        [TestCase(@"\foo\hello.txt", @"c:\some\root\foo\hello.txt")]
        [TestCase(@"\\\foo\hello.txt", @"c:\some\root\foo\hello.txt")]
        [TestCase(@"foo\bar\hello.txt", @"c:\some\root\foo\bar\hello.txt")]
        [TestCase(@"foo\\\\bar\\\\hello.txt", @"c:\some\root\foo\bar\hello.txt")]
        [TestCase(@"..", @"c:\some\root")]
        [TestCase(@"..\..\..", @"c:\some\root")]
        [TestCase(@"foo\..\..", @"c:\some\root")]
        [TestCase(@"foo\..\bar.txt", @"c:\some\root\bar.txt")]
        [TestCase(@"foo\hello\..\..\hello.txt", @"c:\some\root\hello.txt")]
        [TestCase(@"foo\hello\..\hello.txt", @"c:\some\root\foo\hello.txt")]
        public void Normalize(string path, string expected)
        {
            var sandbox = new SandboxDirectory(@"c:\some\root");
            var normalizedPath = sandbox.Normalize(path);

            Assert.That(normalizedPath, Is.EqualTo(expected));
        }
    }
}
