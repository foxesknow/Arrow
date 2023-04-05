using Arrow.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public static class TaskExtensions
    {
        /// <summary>
        /// Configures the task to continue on any context
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ConfiguredTaskAwaitable ContinueOnAnyContext(this Task task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            return task.ConfigureAwait(false);
        }

        /// <summary>
        /// Configures the task to continue on any context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static ConfiguredTaskAwaitable<T> ContinueOnAnyContext<T>(this Task<T> task)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));
            return task.ConfigureAwait(false);
        }

        /// <summary>
        /// Configures the task to continue on any context
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredValueTaskAwaitable ContinueOnAnyContext(this ValueTask task)
        {
            return task.ConfigureAwait(false);
        }

        /// <summary>
        /// Configures the task to continue on any context
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <returns></returns>
        public static ConfiguredValueTaskAwaitable<T> ContinueOnAnyContext<T>(this ValueTask<T> task)
        {
            return task.ConfigureAwait(false);
        }

        /// <summary>
        /// Returns a task that will timeout after a period of time.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TimeoutException">Thrown if the timeout expires</exception>
        public static Task TimeoutAfter(this Task task, TimeSpan timeout)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            return DoTimeoutAfter(task, timeout);

            static async Task DoTimeoutAfter(Task task, TimeSpan timeout)
            {
                if (await task.TryWaitFor(timeout).ContinueOnAnyContext() == false)
                {
                    throw new TimeoutException("the operation has timed out");
                }
            }
        }

        /// <summary>
        /// Returns a task that will timeout after a period of time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="TimeoutException"></exception>
        public static Task<T> TimeoutAfter<T>(this Task<T> task, TimeSpan timeout)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            return DoTimeoutAfter(task, timeout);

            static async Task<T> DoTimeoutAfter(Task<T> task, TimeSpan timeout)
            {
                var result = await task.TryWaitFor(timeout).ContinueOnAnyContext();

                if (result.Success)
                {
                    return result.Value;
                }
                else
                {
                    throw new TimeoutException("the operation has timed out");
                }
            }
        }

        /// <summary>
        /// Attempts to wait for a task to complete
        /// </summary>
        /// <param name="task"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static Task<bool> TryWaitFor(this Task task, TimeSpan timeout)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            return DoTimeoutAfter(task, timeout);

            static async Task<bool> DoTimeoutAfter(Task task, TimeSpan timeout)
            {
                using (var cts = new CancellationTokenSource())
                {
                    var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token)).ContinueOnAnyContext();
                    if (completedTask == task)
                    {
                        // Stop the delay...
                        cts.Cancel();

                        // ..and propogate the result
                        await task;
                        return true;
                    }
                    else
                    {
                        // We timed out
                        return false;
                    }
                }
            }
        }

        public static Task<Result<T>> TryWaitFor<T>(this Task<T> task, TimeSpan timeout)
        {
            if (task is null) throw new ArgumentNullException(nameof(task));

            return DoTimeoutAfter(task, timeout);

            static async Task<Result<T>> DoTimeoutAfter(Task<T> task, TimeSpan timeout)
            {
                using (var cts = new CancellationTokenSource())
                {
                    var completedTask = await Task.WhenAny(task, Task.Delay(timeout, cts.Token)).ContinueOnAnyContext();
                    if (completedTask == task)
                    {
                        // Stop the delay...
                        cts.Cancel();

                        // ..and propogate the result
                        var value = await task;
                        return Result.Success(value);
                    }
                    else
                    {
                        // We timed out
                        return Result.Fail<T>();
                    }
                }
            }
        }
    }
}
