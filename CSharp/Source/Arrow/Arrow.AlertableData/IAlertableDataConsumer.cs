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
        /// <param name="key">The item to read</param>
        /// <param name="state">And state data to pass to the reader</param>
        /// <param name="reader">A function that will take the data and transform it into a result</param>
        /// <returns>(true, result) if the data existed and the reader was called, otherwise (false, default)</returns>
        public (bool Succeeded, TResult Data) TryRead<TState, TResult>(TKey key, TState state, DataReader<TKey, TState, TData, TResult> reader);
        
        /// <summary>
        /// Attempts to read alertable data.
        /// NOTE: The consumer is not reentrant, so do not call back into it from the dataReader
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="key">The item to read</param>
        /// <param name="state">And state data to pass to the reader</param>
        /// <param name="result">A reference to where the reader should store the result</param>
        /// <param name="reader">A function that will take the data and transform it into a result</param>
        /// <returns>True if the data existed and the reader was called, otherwise false</returns>
        public bool TryReadByRef<TState, TResult>(TKey key, TState state, ref TResult result, DataReaderByRef<TKey, TState, TData, TResult> reader);

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
