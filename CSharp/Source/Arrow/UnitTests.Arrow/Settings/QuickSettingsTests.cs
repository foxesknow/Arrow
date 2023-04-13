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
    public class QuickSettingsTests
    {
        [Test]
        public void Initialization()
        {
            var settings = new QuickSettings(("Name", "Jack"), ("Age", 42));

            Assert.That(settings.TryGetSetting("name", out var name), Is.True);
            Assert.That(name, Is.EqualTo("Jack"));

            Assert.That(settings.TryGetSetting("age", out var age), Is.True);
            Assert.That(age, Is.EqualTo(42));

            Assert.That(settings.TryGetSetting("location", out var location), Is.False);
            Assert.That(location, Is.Null);
        }
    }
}
