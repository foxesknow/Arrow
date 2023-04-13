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
    public class DisposableTests
    {
        [Test]
        public void Cons_BadHead()
        {
            IDisposable head = null;
            Assert.Catch(() => Disposable.Cons(head, Disposable.Null));
        }

        [Test]
        public void Cons_BadTail()
        {
            Assert.Catch(() => Disposable.Cons(Disposable.Null, null));
        }

        [Test]
        public void HeadDisposed()
        {
            var headDisposed = false;
            var disposer = Disposer.Make(() => headDisposed = true); 
            var head = Disposable.Cons(disposer, Disposable.Null);

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
            var head = Disposable.Cons(Disposable.Null, disposer);

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
            Assert.Catch(() => Disposable.Cons(head, Disposable.NullAsync));
        }

        [Test]
        public void AsyncCons_BadTail()
        {
            Assert.Catch(() => Disposable.Cons(Disposable.NullAsync, null));
        }

        [Test]
        public async Task AsyncHeadDisposed()
        {
            var headDisposed = false;
            var disposer = AsyncDisposer.Make(() => headDisposed = true); 
            var head = Disposable.Cons(disposer, Disposable.NullAsync);

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
            var head = Disposable.Cons(Disposable.NullAsync, disposer);

            Assert.That(tailDisposed, Is.False);
            await head.DisposeAsync();
            Assert.That(tailDisposed, Is.True);

            // If we try again nothing should happen
            tailDisposed = false;
            await head.DisposeAsync();
            Assert.That(tailDisposed, Is.False);
        }

        [Test]
        public async Task SyncToAsync()
        {
            string name = null;

            var disposer = Disposer.Make(() => name = "Jack");
            Assert.That(name, Is.Null);

            var asyncDisposer = Disposable.SyncToAsync(disposer);
            Assert.That(name, Is.Null);

            await asyncDisposer.DisposeAsync();
            Assert.That(name, Is.EqualTo("Jack"));
        }
    }
}
