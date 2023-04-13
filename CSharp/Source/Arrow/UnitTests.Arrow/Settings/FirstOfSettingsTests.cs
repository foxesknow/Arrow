using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Text;

using NUnit.Framework;

namespace UnitTests.Arrow.Settings
{
    [TestFixture]
    public class FirstOfSettingsTests
    {
        [Test]
        public void JustOneSetting()
        {
            var text = TokenExpander.ExpandText("${first-of:guid:app}");
            Assert.That(text, Is.Not.Null & Has.Length.GreaterThan(0));
        }

        [Test]
        public void JustOneSetting_NoSuchNamespace()
        {
            // This throws an exception as the setting provider ultimately returns null
            Assert.Catch(() => TokenExpander.ExpandText("${first-of:DoesNotExist:app}"));
        }

        [Test]
        public void TwoSettings_OneHasMissingNamespace()
        {
            var text = TokenExpander.ExpandText("${first-of: DoesNotExist:app ?? guid:app}");
            Assert.That(text, Is.Not.Null & Has.Length.GreaterThan(0));
        }
    }
}
