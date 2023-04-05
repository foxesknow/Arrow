using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Useful Task related methods that are not on Task
    /// </summary>
    public static partial class TaskEx
    {
        /// <summary>
        /// Requests a cancel and waits on a task that depends on the cancel.
        /// Any exception thrown by the task is swallowed
        /// </summary>
        /// <param name="cts"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static async Task CancelAndWait(CancellationTokenSource cts, Task task)
        {
            if (cts is null) throw new ArgumentNullException(nameof(cts));
            if (task is null) throw new ArgumentNullException(nameof(task));

            cts.Cancel();

            try
            {
                await task.ContinueOnAnyContext();
            }
            catch (TaskCanceledException)
            {
                // We can ignore it
            }
        }
    }
}
