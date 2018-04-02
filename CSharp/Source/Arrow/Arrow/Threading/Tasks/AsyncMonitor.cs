using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// An asynchronous monitor
    /// </summary>
    public class AsyncMonitor
    {
        private readonly AsyncSemaphore m_Lock = new AsyncSemaphore(1);
        private readonly Task<Releaser> m_Releaser;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public AsyncMonitor()
        {
            m_Releaser = Task.FromResult(new Releaser(m_Lock));
        }

        /// <summary>
        /// Returns a task that will acquire the lock.
        /// You should use this in a using() block so that the lock is released when finished with
        /// </summary>
        /// <returns></returns>
        public Task<Releaser> LockAsync()
        {
            var task = m_Lock.WaitAsync();

            if(task.IsCompleted)
            {
                // We've acquired the lock, so return the releaser
                return m_Releaser;
            }
            else
            {
                // We couldn't acquire the lock, so take the waiting task
                // and have it return a releaser once the wait is complete
                return task.ContinueWith((t, state) =>
                {
                    return new Releaser((AsyncSemaphore)state);
                }, m_Lock, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            }
        }

        public struct Releaser : IDisposable
        {
            private AsyncSemaphore m_Lock;

            internal Releaser(AsyncSemaphore @lock)
            {
                m_Lock = @lock;
            }

            public void Dispose()
            {
                if(m_Lock != null)
                {
                    m_Lock.Release();
                    m_Lock = null;
                }
            }
        }
    }
}
