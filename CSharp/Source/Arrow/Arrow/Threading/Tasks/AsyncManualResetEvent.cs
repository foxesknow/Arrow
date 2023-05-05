using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// An asynchronous manual reset event.
    /// Multiple threads can wait on the event, and all will be released when the event is signaled
    /// </summary>
    public sealed class AsyncManualResetEvent : AsyncEventWaitHandle
    {
        private volatile TaskCompletionSource<bool> m_Source = MakeTcs();
        private bool m_Disposed;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="initiallySignaled">true if the event is initially signaled, false to set the event to nonsignaled</param>
        public AsyncManualResetEvent(bool initiallySignaled)
        {
            if(initiallySignaled)
            {
                m_Source.SetResult(true);
            }
        }

        public override void Dispose()
        {
            m_Disposed = true;

            var source = m_Source;
            source.TrySetException(new ObjectDisposedException(nameof(AsyncManualResetEvent)));
        }

        /// <summary>
        /// Sets the event, waking any threads waiting on the event
        /// </summary>
        public override void Set()
        {
            ThrowIfDisposed();

            var source = m_Source;
            source.TrySetResult(true);
        }

        /// <summary>
        /// Waits for the event to be signaled
        /// </summary>
        /// <returns></returns>
        public override Task WaitAsync()
        {
            ThrowIfDisposed();

            return m_Source.Task;
        }

        /// <summary>
        /// Resets the event back to unsignaled
        /// </summary>
        public override void Reset()
        {
            while(true)
            {
                var tcs = m_Source;
                var disposed = m_Disposed;

                if(tcs.Task.IsCompleted == false)
                {
                    if(disposed) tcs.TrySetException(new ObjectDisposedException(nameof(AsyncManualResetEvent)));
                    break;
                }

                if(Interlocked.CompareExchange(ref m_Source, MakeTcs(), tcs) == tcs)
                {
                    if(disposed) m_Source.TrySetException(new ObjectDisposedException(nameof(AsyncManualResetEvent)));
                    break;
                }
            }

            // NOTE: We do the throw here so that the TCS in the look gets the exception attached
            // before we throw back to the caller. That will wake up anyone awaiting on a task
            // and propagate the exception to them.
            ThrowIfDisposed();
        }

        private void ThrowIfDisposed()
        {
            if(m_Disposed) throw new ObjectDisposedException(nameof(AsyncManualResetEvent));
        }
    }
}
