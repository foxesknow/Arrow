using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading
{
    public static class TaskExtensions
    {
        public static ConfiguredTaskAwaitable ContinueOnAnyContext(this Task task)
        {
            if(task is null) throw new ArgumentNullException(nameof(task));
            return task.ConfigureAwait(false);
        }

        public static ConfiguredTaskAwaitable<T> ContinueOnAnyContext<T>(this Task<T> task)
        {
            if(task is null) throw new ArgumentNullException(nameof(task));
            return task.ConfigureAwait(false);
        }
    }
}
