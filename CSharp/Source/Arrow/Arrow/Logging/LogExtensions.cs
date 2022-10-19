using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

#nullable enable

namespace Arrow.Logging
{
    /// <summary>
    /// Useful logging extensions
    /// </summary>
    public static class LogExtensions
    {
        /// <summary>
        /// Pushes properties into the current log context, if supported
        /// </summary>
        /// <param name="log"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static IDisposable Push(this ILog log, params (string Name, object? value)[] properties)
        {
            return DoPush(log, properties);
        }

        /// <summary>
        /// Pushes properties into the current log context, if supported
        /// </summary>
        /// <param name="log"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static IDisposable Push(this ILog log, IEnumerable<(string Name, object? value)> properties)
        {
            return DoPush(log, properties);
        }

        private static IDisposable DoPush(ILog log, IEnumerable<(string Name, object? value)> properties)
        {
            if(log is null) return NullDisposable.Instance;
            if(properties is null) return NullDisposable.Instance;

            if(log is IPropertyContext context)
            {
                var pusher = context.GetPusher();
                if(pusher is not null)
                {
                    var disposer = pusher.Push(properties);
                    return disposer ?? NullDisposable.Instance;
                }
            }

            return NullDisposable.Instance;
        }
    }
}
