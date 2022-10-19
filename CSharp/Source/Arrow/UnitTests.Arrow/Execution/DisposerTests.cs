using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

using NUnit.Framework;

namespace UnitTests.Arrow.Execution
{
    [TestFixture]
    public class DisposerTests
    {
        [Test]
        public void Initialization()
        {
            Assert.Catch(() => Disposer.Make(null!));
        }

        [Test]
        public void OnlyCalledOnce()
        {
            var count = 0;
            
            IDisposable disposer = new Disposer(() => count++);
            Assert.That(count, Is.EqualTo(0));

            disposer.Dispose();
            Assert.That(count, Is.EqualTo(1));

            disposer.Dispose();
            Assert.That(count, Is.EqualTo(1));
        }
    }
}
