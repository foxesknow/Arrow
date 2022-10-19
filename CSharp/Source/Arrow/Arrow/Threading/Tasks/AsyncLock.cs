using Arrow.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
    public sealed class AsyncLock
    {
        private readonly AsyncSemaphore m_Lock = new(1);

        public TaskAwaiter<IDisposable> GetAwaiter()
        {
            var t = WaitFor();
            return t.GetAwaiter();
        }

        public async Task<IDisposable> WaitFor()
        {
            await m_Lock.WaitAsync().ContinueOnAnyContext();
            return new Releaser(this);
        }

        public IDisposable TryLock(out bool lockAcquired)
        {
            if(m_Lock.TryAccquire())
            {
                lockAcquired = true;
                return new Releaser(this);
            }

            lockAcquired = false;
            return NullDisposable.Instance;
        }

        private sealed class Releaser : IDisposable
        {
            private AsyncLock? m_ToRelease;

            public Releaser(AsyncLock toRelease)
            {
                m_ToRelease = toRelease;
            }

            public void Dispose()
            {
                if(m_ToRelease is not null)
                {
                    m_ToRelease.m_Lock.Release();
                    m_ToRelease = null;
                }
            }
        }
    }
}
