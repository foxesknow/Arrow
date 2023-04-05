using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Settings;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Settings
{
    [TestFixture]
    public class EnvironmentSettingsTests
    {
        [Test]
        public void Test()
        {
            ISettings p = CreateProvider();

            p.TryGetSetting("username", out var value);
            Assert.IsNotNull(value);
            Assert.That(value.ToString().Length, Is.GreaterThan(0));

            p.TryGetSetting("foo", out value);
            Assert.IsNull(value);
        }

        internal static ISettings CreateProvider()
        {
            return EnvironmentSettings.Instance;
        }
    }
}
