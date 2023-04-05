using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks.Streaming
{
    /// <summary>
    /// Defines the behaviour of a publisher
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public interface IPublisher<in TData>
    {
        /// <summary>
        /// Publisher data
        /// </summary>
        /// <param name="data"></param>
        void Publish(TData data);
    }
}
