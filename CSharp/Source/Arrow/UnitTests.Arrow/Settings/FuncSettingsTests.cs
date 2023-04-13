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
    public class FuncSettingsTests
    {
        [Test]
        public void Lookup()
        {
            var settings = new FuncSettings(GetSettings);

            Assert.That(settings.TryGetSetting("Name", out var name), Is.True);
            Assert.That(name, Is.EqualTo("Jack"));

            Assert.That(settings.TryGetSetting("Location", out var location), Is.False);
            Assert.That(location, Is.Null);
        }

        private static object? GetSettings(string name)
        {
            return name.ToLower() switch
            {
                "name"  => "Jack",
                "age"   => 42,
                _       => null
            };
        }
    }
}
