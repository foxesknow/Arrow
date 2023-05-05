using Arrow.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// An asynchronous lock, similar to a .NET monitor.
    /// To acquire the monitor use a using() block to ensure the lock is released.
    /// 
    /// NOTE: This lock IS NOT recursive
    /// </summary>
    /// <example>
    /// using(await syncRoot)
    /// {
    ///     // Do something
    /// }
    /// </example>
    public sealed class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim m_Lock = new(1);
        private bool m_Disposed;

        public TaskAwaiter<Releaser> GetAwaiter()
        {
            ThrowIfDisposed();

            var t = WaitFor();
            return t.GetAwaiter();
        }

        public Task<Releaser> WaitFor(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();
            return Execute(cancellationToken);

            async Task<Releaser> Execute(CancellationToken cancellationToken)
            {
                await m_Lock.WaitAsync(cancellationToken).ContinueOnAnyContext();
                return new Releaser(this);
            }
        }

        /// <summary>
        /// Disposes of the lock
        /// </summary>
        public void Dispose()
        {
            if(m_Disposed == false)
            {
                m_Lock.Dispose();
                m_Disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if(m_Disposed) throw new ObjectDisposedException(nameof(AsyncLock));
        }

        public readonly struct Releaser : IDisposable
        {
            private readonly AsyncLock? m_ToRelease;

            public Releaser(AsyncLock toRelease)
            {
                m_ToRelease = toRelease;
            }

            public void Dispose()
            {
                if(m_ToRelease is not null)
                {
                    m_ToRelease.m_Lock.Release();
                }
            }
        }
    }
}
