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
    public class AsyncDisposerTests
    {
        [Test]
        public void Initialization()
        {
            Assert.Catch(() => AsyncDisposer.Make(null!));
        }

        [Test]
        public async Task OnlyCalledOnce()
        {
            var count = 0;
            
            IAsyncDisposable disposer = new AsyncDisposer(() => 
            {
                count++;
                return default;
            });

            Assert.That(count, Is.EqualTo(0));

            await disposer.DisposeAsync();
            Assert.That(count, Is.EqualTo(1));

            await disposer.DisposeAsync();
            Assert.That(count, Is.EqualTo(1));
        }
    }
}
