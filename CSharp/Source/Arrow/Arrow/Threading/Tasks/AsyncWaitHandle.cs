using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        protected static TaskCompletionSource<bool> MakeTcs()
        {
            return new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        /// <summary>
        /// Returns a task that will be completed at some point
        /// </summary>
        /// <returns></returns>
        public abstract Task WaitAsync();

        /// <summary>
        /// Allows a wait handle to be used with the await keyword
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter GetAwaiter()
        {
            return WaitAsync().GetAwaiter();
        }
     
        protected void ReleaseTcs(TaskCompletionSource<bool> tcs)
        {
            Task.Factory.StartNew
            (
                s => ((TaskCompletionSource<bool>)s!).TrySetResult(true),
                tcs,
                CancellationToken.None,
                TaskCreationOptions.PreferFairness,
                TaskScheduler.Default
            );

            tcs.Task.Wait();
        }
    }
}
