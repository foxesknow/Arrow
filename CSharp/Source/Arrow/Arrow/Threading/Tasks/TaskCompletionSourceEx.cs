using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    /// <summary>
    /// Useful TaskCompletionSource methods
    /// </summary>
    public static class TaskCompletionSourceEx
    {
        /// <summary>
        /// Creates a TaskCompletionSource that will run the continuation asynchronously on another thread
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static TaskCompletionSource<T> CreateAsynchronousCompletionSource<T>()
        {
            return new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }
}
