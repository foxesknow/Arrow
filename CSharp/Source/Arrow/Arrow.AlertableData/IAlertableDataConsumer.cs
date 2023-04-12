using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// Provides access to alertable data,
    /// </summary>
    /// <remarks>
    /// When accessing data you should not await anything.
    /// You should also do as little processing as possible to minimize lock contention.
    /// </remarks>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public interface IAlertableDataConsumer<TKey, TData> where TData : class 
                                                         where TKey : notnull
    {
        /// <summary>
        /// Attempts to read alertable data.
        /// NOTE: The consumer is not reentrant, so do not call back into it from the dataReader
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public (bool Succeeded, TResult Data) TryRead<TState, TResult>(TKey key, TState state, DataReader<TKey, TState, TData, TResult> reader);

        /// <summary>
        /// Reads from the consumer.
        /// NOTE: You MUST call Dispose() on the returned structure when you have finished reading.
        /// DO NOT hold onto the data. It can only be accessed before you call Dispose()
        /// </summary>
        /// <param name="key"></param>
        /// <param name="readResult"></param>
        /// <returns></returns>
        public ReadLock Read(TKey key, out ReadResult<TData> readResult);

        /// <summary>
        /// Checks to see if a key is subscribed
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool IsSubscribed(TKey key);
    }
}
