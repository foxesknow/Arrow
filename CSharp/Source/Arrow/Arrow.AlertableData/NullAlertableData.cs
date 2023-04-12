using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// An implementation of alertable data that does not store anything
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public sealed class NullAlertableData<TKey, TData> :
        IAlertableDataConsumer<TKey, TData>,
        IAlertableDataProducer<TKey, TData>
        where TData : class where TKey : notnull
    {
        /// <inheritdoc/>
        public bool IsSubscribed(TKey key)
        {
            return false;
        }

        /// <inheritdoc/>
        public bool Publish<TState>(TKey key, TState state, DataWriter<TKey, TState, TData> writer)
        {
            return false;
        }

        /// <inheritdoc/>
        public ReadLock Read(TKey key, out ReadResult<TData> readResult)
        {
            readResult = default;
            return default;
        }

        /// <inheritdoc/>
        public (bool Succeeded, TResult Data) TryRead<TState, TResult>(TKey key, TState state, DataReader<TKey, TState, TData, TResult> reader)
        {
            return (false, default!);
        }
    }
}
