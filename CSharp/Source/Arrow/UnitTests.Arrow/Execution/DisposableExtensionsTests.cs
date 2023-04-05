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
            Assert.Catch(() => head.Cons(Disposable.Null));
        }

        [Test]
        public void Cons_BadTail()
        {
            Assert.Catch(() => Disposable.Null.Cons(null));
        }

        [Test]
        public void HeadDisposed()
        {
            var headDisposed = false;
            var disposer = Disposer.Make(() => headDisposed = true); 
            var head = disposer.Cons(Disposable.Null);

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
            var head = Disposable.Null.Cons(disposer);

            Assert.That(tailDisposed, Is.False);
            head.Dispose();
            Assert.That(tailDisposed, Is.True);

            // If we try again nothing should happen
            tailDisposed = false;
            head.Dispose();
            Assert.That(tailDisposed, Is.False);
        }


        [Test]
        public void AsyncCons_BadHead()
        {
            IAsyncDisposable head = null;
            Assert.Catch(() => head.Cons(Disposable.NullAsync));
        }

        [Test]
        public void AsyncCons_BadTail()
        {
            Assert.Catch(() => Disposable.NullAsync.Cons(null));
        }

        [Test]
        public async Task AsyncHeadDisposed()
        {
            var headDisposed = false;
            var disposer = AsyncDisposer.Make(() => headDisposed = true); 
            var head = disposer.Cons(Disposable.NullAsync);

            Assert.That(headDisposed, Is.False);
            await head.DisposeAsync();
            Assert.That(headDisposed, Is.True);

            // If we try again nothing should happen
            headDisposed = false;
            await head.DisposeAsync();
            Assert.That(headDisposed, Is.False);
        }

        [Test]
        public async Task AsyncTailDisposed()
        {
            var tailDisposed = false;
            var disposer = AsyncDisposer.Make(() => tailDisposed = true); 
            var head = Disposable.NullAsync.Cons(disposer);

            Assert.That(tailDisposed, Is.False);
            await head.DisposeAsync();
            Assert.That(tailDisposed, Is.True);

            // If we try again nothing should happen
            tailDisposed = false;
            await head.DisposeAsync();
            Assert.That(tailDisposed, Is.False);
        }
    }
}
