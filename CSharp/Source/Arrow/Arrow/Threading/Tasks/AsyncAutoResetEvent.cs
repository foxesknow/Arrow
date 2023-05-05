using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// An asynchronous auto reset event.
    /// Multiple threads can wait on the event, but only one will be released when the event is signaled
    /// </summary>
    public sealed class AsyncAutoResetEvent : AsyncEventWaitHandle
    {
        private bool m_Signaled;
        private readonly Queue<TaskCompletionSource<bool>> m_PendingWaits = new();

        private bool m_Disposed;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="initiallySignaled">true if the event is initially signaled, false to set the event to nonsignaled</param>
        public AsyncAutoResetEvent(bool initiallySignaled)
        {
            m_Signaled = initiallySignaled;
        }

        public override void Dispose()
        {
            lock(m_PendingWaits)
            {
                if(m_Disposed) return;

                // Abort any pending wait
                if(m_PendingWaits.Count != 0)
                {
                    var exception = new ObjectDisposedException(nameof(AsyncAutoResetEvent));

                    while(m_PendingWaits.TryDequeue(out var tcs))
                    {
                        tcs.TrySetException(exception);
                    }
                }

                m_Disposed = true;
            }
        }

        /// <summary>
        /// Sets the event back to unsignaled
        /// </summary>
        public override void Reset()
        {
            lock(m_PendingWaits)
            {
                ThrowIfDisposed();
                
                m_Signaled = false;
            }
        }

        /// <summary>
        /// Signals the event.
        /// If anyone is waiting then they will be released and the event will be reset,
        /// otherwise the event will stay signaled until someone waits on it
        /// </summary>
        public override void Set()
        {
            TaskCompletionSource<bool>? tcs = null;

            lock(m_PendingWaits)
            {
                ThrowIfDisposed();

                if(m_PendingWaits.Count > 0)
                {
                    // There's someone waiting, so just release them
                    tcs = m_PendingWaits.Dequeue();
                }
                else
                {
                    // Nobody is waiting, so remember we're signaled
                    m_Signaled = true;
                }
            }

            // If there was someone waiting then release them
            if(tcs is not null)
            {
                tcs.TrySetResult(true);
            }
        }

        public override Task WaitAsync()
        {
            lock(m_PendingWaits)
            {
                ThrowIfDisposed();

                if(m_Signaled)
                {
                    // Easy, we've been signaled so just return success
                    m_Signaled = false;
                    return Task.CompletedTask;
                }
                else
                {
                    // We've not been signaled yet, so place the caller on a queue
                    // waiting to be signaled
                    var source = MakeTcs();
                    m_PendingWaits.Enqueue(source);

                    return source.Task;
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if(m_Disposed) throw new ObjectDisposedException(nameof(AsyncAutoResetEvent));
        }
    }
}
