using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application.DaemonHosting;

using NUnit.Framework;

namespace UnitTests.Arrow.Application.DaemonHosting
{
    [TestFixture]
    public class InteractiveCommandTests
    {
        [Test]
        public void Initialize_InvalidName()
        {
            Assert.Catch(() => new InteractiveCommand(null, "description", Handler));
            Assert.Catch(() => new InteractiveCommand("", "description", Handler));
            Assert.Catch(() => new InteractiveCommand("   ", "description", Handler));
        }

        [Test]
        public void Initialize_InvalidDescription()
        {
            Assert.Catch(() => new InteractiveCommand("dir", null, Handler));
            Assert.Catch(() => new InteractiveCommand("dir", "", Handler));
            Assert.Catch(() => new InteractiveCommand("fir", "    ", Handler));
        }

        [Test]
        public void Initialize_InvalidHanlder()
        {
            Assert.Catch(() => new InteractiveCommand("dir", "description", null));
        }

        [Test]
        public void DisplayName()
        {
            var command = new InteractiveCommand("dir", "description", Handler);
            Assert.That(command.DisplayName(), Is.Not.Null & Has.Length.GreaterThan(0));
        }

        private IEnumerable<object> Handler(PipelinePart part)
        {
            return Enumerable.Empty<object>();
        }
    }
}
