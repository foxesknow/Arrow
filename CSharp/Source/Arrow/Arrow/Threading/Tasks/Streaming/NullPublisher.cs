using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks.Streaming
{
    /// <summary>
    /// A publisher that does nothing with its data.
    /// </summary>
    public sealed class NullPublisher : IPublisher<object?>
    {
        public void Publish(object? data)
        {
            // Does nothing
        }
    }

    /// <summary>
    /// A publisher that does nothing with its data.
    /// </summary>
    public sealed class NullPublisher<TData> : IPublisher<TData>
    {
        public void Publish(TData data)
        {
            // Does nothing
        }
    }
}
