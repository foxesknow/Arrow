using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// Publishes alertable data
    /// </summary>
    /// <remarks>
    /// When accessing data you should not await anything.
    /// You should also do as little processing as possible to minimize lock contention.
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public interface IAlertableDataProducer<TKey, TData> : IAlertableDataConsumer<TKey, TData> where TData : class 
                                                                                               where TKey : notnull
    {
        /// <summary>
        /// Publishes data.
        /// The publisher is not reentrant so do not call any methods on it from within the writer callback
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <param name="writer"></param>
        /// <returns></returns>
        public bool Publish<TState>(TKey key, TState state, DataWriter<TKey, TState, TData> writer);
    }
}
