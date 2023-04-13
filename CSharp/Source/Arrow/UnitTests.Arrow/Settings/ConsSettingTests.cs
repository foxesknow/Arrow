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
    public class ConsSettingTests
    {
        [Test]
        public void Pair()
        {
            var head = new QuickSettings(("Name", "Jack"));
            var tail = new QuickSettings(("Name", "Sawyer"), ("Age", 44));

            var cons = new ConsSetting(head, tail);

            Assert.That(cons.TryGetSetting("name", out var name), Is.True);
            Assert.That(name, Is.EqualTo("Jack"));

            Assert.That(cons.TryGetSetting("age", out var age), Is.True);
            Assert.That(age, Is.EqualTo(44));
        }
    }
}
