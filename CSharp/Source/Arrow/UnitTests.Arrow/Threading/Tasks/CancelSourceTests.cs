using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;

using NUnit.Framework;

namespace UnitTests.Arrow.Threading.Tasks
{
    [TestFixture]
    public class CancelSourceTests
    {
        [Test]
        public void CanceledTaskIsCancelled()
        {
            using(var source = new CancelSource())
            {
                source.Cancel();
                Assert.CatchAsync<OperationCanceledException>(async () => await source.CanceledTask);
                Assert.That(source.CanceledTask.IsCompletedSuccessfully, Is.False);
            }
        }

        [Test]
        public void CompletedTaskIsCancelled()
        {
            using(var source = new CancelSource())
            {
                Assert.That(source.CompletedTask.IsCompleted, Is.False);
                source.Cancel();
                Assert.That(source.CompletedTask.IsCompleted, Is.True);
                Assert.That(source.CompletedTask.IsCompletedSuccessfully, Is.True);
            }
        }

        [Test]
        public void CancellationTokenSourceIsCancelled()
        {
            using(var source = new CancelSource())
            {
                source.Cancel();
                Assert.That(source.Token.IsCancellationRequested, Is.True);
            }
        }
    }
}
