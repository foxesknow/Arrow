using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Settings;

using NUnit.Framework;

namespace UnitTests.Arrow.Settings
{
    [TestFixture]
    public class SlurpSettingsTests
    {
        [Test]
        public void FileIsOptional()
        {
            var settings = new SlurpSettings();
            Assert.That(settings.TryGetSetting(@"c:\does\not\exist.txt?", out var value), Is.False);
            Assert.That(value, Is.Null);
        }

        [Test]
        public void FileIsMissing()
        {
            var settings = new SlurpSettings();
            Assert.Throws<IOException>(() => settings.TryGetSetting(@"c:\does\not\exist.txt", out var value));
        }

        [Test]
        public void FileExists()
        {
            var filename = Path.GetTempFileName();

            try
            {
                File.WriteAllText(filename, "HelloWorld");
                
                var settings = new SlurpSettings();
                Assert.That(settings.TryGetSetting(filename, out var value), Is.True);
                Assert.That(value, Is.EqualTo("HelloWorld"));
            }
            finally
            {
                File.Delete(filename);
            }
        }
    }
}
