using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Base class for anything that functions like an awaitable handle
    /// </summary>
    public abstract class AsyncWaitHandle
    {
        /// <summary>
        /// A task that has always completed successfully
        /// </summary>
        protected static readonly Task Completed = Task.FromResult(true);

        /// <summary>
        /// Returns a task that will be completed at some point
        /// </summary>
        /// <returns></returns>
        public abstract Task WaitAsync();

        protected void SetTaskCompletionSouce(TaskCompletionSource<bool> source)
        {
            Task.Factory.StartNew
            (
                s => ((TaskCompletionSource<bool>)s).TrySetResult(true),
                source, 
                CancellationToken.None, 
                TaskCreationOptions.PreferFairness, 
                TaskScheduler.Default
            );
            
            source.Task.Wait(); 
        }
    }
}
