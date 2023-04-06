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
    public class DaemonBaseTests
    {
        [Test]
        public void HasCommands()
        {
            var daemon = new TestDaemon();
            foreach(var command in daemon.AllCommands())
            {
                Assert.That(command, Is.Not.Null);
            }
        }

        [Test]
        public void HasFilters()
        {
            var daemon = new TestDaemon();
            foreach(var filter in daemon.AllFilters())
            {
                Assert.That(filter, Is.Not.Null);
            }
        }

        private class TestDaemon : DaemonBase
        {
            protected internal override void StartDaemon(string[] args)
            {
            }

            protected internal override void StopDaemon()
            {
            }

            public IEnumerable<InteractiveCommand> AllCommands()
            {
                return this.GetCommands();
            }

            public IEnumerable<InteractiveFilter> AllFilters()
            {
                return this.GetFilters();
            }
        }
    }
}
