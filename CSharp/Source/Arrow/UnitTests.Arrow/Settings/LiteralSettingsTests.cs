using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Settings;
using Arrow.Text;

using NUnit.Framework;

namespace UnitTests.Arrow.Settings
{
    [TestFixture]
    public class LiteralSettingsTests
    {
        [Test]
        public void ReturnsSettingName()
        {
            var setting = new LiteralSettings();
            Assert.That(setting.TryGetSetting("hello world", out var value), Is.True);
            Assert.That(value, Is.EqualTo("hello world"));
        }

        [Test]
        public void UsedWithFirstOf()
        {
            var text = TokenExpander.ExpandText("${first-of: Foo:bar ?? literal:hello}");
            Assert.That(text, Is.EqualTo("hello"));
        }
    }
}
