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
    public class DisposableExtensionsTests
    {
        [Test]
        public void Cons_BadHead()
        {
            IDisposable head = null;
            Assert.Catch(() => head.Cons(NullDisposable.Instance));
        }

        [Test]
        public void Cons_BadTail()
        {
            Assert.Catch(() => NullDisposable.Instance.Cons(null));
        }

        [Test]
        public void HeadDisposed()
        {
            var headDisposed = false;
            var disposer = Disposer.Make(() => headDisposed = true); 
            var head = disposer.Cons(NullDisposable.Instance);

            Assert.That(headDisposed, Is.False);
            head.Dispose();
            Assert.That(headDisposed, Is.True);

            // If we try again nothing should happen
            headDisposed = false;
            head.Dispose();
            Assert.That(headDisposed, Is.False);
        }

        [Test]
        public void TailDisposed()
        {
            var tailDisposed = false;
            var disposer = Disposer.Make(() => tailDisposed = true); 
            var head = NullDisposable.Instance.Cons(disposer);

            Assert.That(tailDisposed, Is.False);
            head.Dispose();
            Assert.That(tailDisposed, Is.True);

            // If we try again nothing should happen
            tailDisposed = false;
            head.Dispose();
            Assert.That(tailDisposed, Is.False);
        }
    }
}
