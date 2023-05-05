using Arrow.Calendar;
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
    /// An asynchronous semaphore
    /// </summary>
    public sealed class AsyncSemaphore : IDisposable
    {
        private readonly SemaphoreSlim m_Semaphore;
        private bool m_Disposed;

        /// <summary>
        /// Initializes the semaphore.
        /// The initial count will be set to the max count
        /// </summary>
        /// <param name="maxCount"></param>
        /// <exception cref="ArgumentException"></exception>
        public AsyncSemaphore(int maxCount) : this(maxCount, maxCount)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="initialCount"></param>
        /// <param name="maxCount"></param>
        public AsyncSemaphore(int initialCount, int maxCount)
        {
            m_Semaphore = new(initialCount, maxCount);
        }

        /// <summary>
        /// Disposes of the semaphore
        /// </summary>
        public void Dispose()
        {
            if(m_Disposed == false)
            {
                m_Semaphore.Dispose();
                m_Disposed = true;
            }
        }

        /// <summary>
        /// Waits to enter the semaphore
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task WaitAsync(CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return m_Semaphore.WaitAsync(cancellationToken);
        }

        /// <summary>
        /// Waits to enter the semaphore within a given timeframe
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<bool> WaitAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            return m_Semaphore.WaitAsync(timeout, cancellationToken);
        }

        /// <summary>
        /// Allows a semaphore to be used with the await keyword
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter GetAwaiter()
        {
            ThrowIfDisposed();

            return WaitAsync().GetAwaiter();
        }

        /// <summary>
        /// Releases the semaphore once
        /// </summary>
        public void Release()
        {
            ThrowIfDisposed();

            m_Semaphore.Release();
        }

        private void ThrowIfDisposed()
        {
            if(m_Disposed) throw new ObjectDisposedException(nameof(AsyncSemaphore));
        }
    }
}
