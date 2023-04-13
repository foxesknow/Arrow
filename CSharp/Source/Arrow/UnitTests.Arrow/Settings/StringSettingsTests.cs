using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Settings;

using NUnit.Framework;

namespace UnitTests.Arrow.Settings
{
    [TestFixture]
    public class StringSettingsTests
    {
        [Test]
        public void Empty()
        {
            var settings = new StringSettings();
            Assert.That(settings.TryGetSetting("Empty", out var value), Is.True);
            Assert.That(value, Is.EqualTo(""));
        }

        [Test]
        public void Unknown()
        {
            var settings = new StringSettings();
            Assert.That(settings.TryGetSetting("Foo", out var value), Is.False);
            Assert.That(value, Is.Null);
        }
    }
}
