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
    public class ObjectDisposeTests
    {
        [Test]
        public void MultiThreaded_InitialState()
        {
            var flag = ObjectDispose.MultiThreadedNotDisposed;
            Assert.That(ObjectDispose.IsDisposed(ref flag), Is.False);
        }

        [Test]
        public void MultiThreaded_IsDisposed_NotDisposed()
        {
            var flag = ObjectDispose.MultiThreadedNotDisposed;
            Assert.That(ObjectDispose.IsDisposed(ref flag), Is.False);
        }

        [Test]
        public void MultiThreaded_IsDisposed_Disposed()
        {
            var flag = ObjectDispose.MultiThreadedNotDisposed;
            ObjectDispose.TryDispose(ref flag);

            Assert.That(ObjectDispose.IsDisposed(ref flag), Is.True);
        }

        [Test]
        public void MultiThreaded_TryDispose()
        {
            var flag = ObjectDispose.MultiThreadedNotDisposed;
            Assert.That(ObjectDispose.TryDispose(ref flag), Is.True);
            Assert.That(flag, Is.Not.EqualTo(ObjectDispose.MultiThreadedNotDisposed));
        }

        [Test]
        public void MultiThreaded_TryDispose_MultipleCalls()
        {
            var flag = ObjectDispose.MultiThreadedNotDisposed;
            Assert.That(ObjectDispose.TryDispose(ref flag), Is.True);
            Assert.That(ObjectDispose.TryDispose(ref flag), Is.False);
        }

        [Test]
        public void MultiThreaded_ThrowIfDisposed()
        {
            var flag = ObjectDispose.MultiThreadedNotDisposed;
            Assert.DoesNotThrow(() => ObjectDispose.ThrowIfDisposed(ref flag, nameof(ObjectDisposeTests)));
        }

        [Test]
        public void MultiThreaded_ThrowIfDisposed_Disposed()
        {
            var flag = ObjectDispose.MultiThreadedNotDisposed;
            ObjectDispose.TryDispose(ref flag);
            Assert.Throws<ObjectDisposedException>(() => ObjectDispose.ThrowIfDisposed(ref flag, nameof(ObjectDisposeTests)));
        }


        [Test]
        public void SingleThreaded_InitialState()
        {
            var flag = ObjectDispose.SingleThreadedNotDisposed;
            Assert.That(ObjectDispose.IsDisposed(ref flag), Is.False);
        }

        [Test]
        public void SingleThreaded_IsDisposed_NotDisposed()
        {
            var flag = ObjectDispose.SingleThreadedNotDisposed;
            Assert.That(ObjectDispose.IsDisposed(ref flag), Is.False);
        }

        [Test]
        public void SingleThreaded_IsDisposed_Disposed()
        {
            var flag = ObjectDispose.SingleThreadedNotDisposed;
            ObjectDispose.TryDispose(ref flag);

            Assert.That(ObjectDispose.IsDisposed(ref flag), Is.True);
        }

        [Test]
        public void SingleThreaded_TryDispose()
        {
            var flag = ObjectDispose.SingleThreadedNotDisposed;
            Assert.That(ObjectDispose.TryDispose(ref flag), Is.True);
            Assert.That(flag, Is.Not.EqualTo(ObjectDispose.SingleThreadedNotDisposed));
        }

        [Test]
        public void SingleThreaded_TryDispose_SinglepleCalls()
        {
            var flag = ObjectDispose.SingleThreadedNotDisposed;
            Assert.That(ObjectDispose.TryDispose(ref flag), Is.True);
            Assert.That(ObjectDispose.TryDispose(ref flag), Is.False);
        }

        [Test]
        public void SingleThreaded_ThrowIfDisposed()
        {
            var flag = ObjectDispose.SingleThreadedNotDisposed;
            Assert.DoesNotThrow(() => ObjectDispose.ThrowIfDisposed(ref flag, nameof(ObjectDisposeTests)));
        }

        [Test]
        public void SingleThreaded_ThrowIfDisposed_Disposed()
        {
            var flag = ObjectDispose.SingleThreadedNotDisposed;
            ObjectDispose.TryDispose(ref flag);
            Assert.Throws<ObjectDisposedException>(() => ObjectDispose.ThrowIfDisposed(ref flag, nameof(ObjectDisposeTests)));
        }
    }
}
