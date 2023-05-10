using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Provides a series of tasks and tokens that will be set when a cancel is requested
    /// </summary>
    public sealed class CancelSource : IDisposable
    {
        private readonly TaskCompletionSource<bool> m_CompletedTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly TaskCompletionSource<bool> m_CancelTask = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly CancellationTokenSource m_CancellationTokenSource = new();
        
        /// <inheritdoc/>
        public void Dispose()
        {
            m_CancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Returns a token you can check for cancellation
        /// </summary>
        public CancellationToken Token
        {
            get{return m_CancellationTokenSource.Token;}
        }

        /// <summary>
        /// Returns a task that will be cancelled when the source is cancelled
        /// </summary>
        public Task CanceledTask
        {
            get{return m_CancelTask.Task;}
        }

        /// <summary>
        /// Returns a task that will be in the completed state when the source is cancelled
        /// </summary>
        public Task CompletedTask
        {
            get{return m_CompletedTask.Task;}
        }

        public void Cancel()
        {
            m_CancellationTokenSource.Cancel();
            m_CancelTask.TrySetCanceled(m_CancellationTokenSource.Token);
            m_CompletedTask.TrySetResult(true);
        }
    }
}
