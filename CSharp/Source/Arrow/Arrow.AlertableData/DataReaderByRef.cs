using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// A delegate used to read data from an alertable data manager and allows the
    /// caller to populate the result via a reference, which pay give a performance improvement.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="key"></param>
    /// <param name="state"></param>
    /// <param name="data"></param>
    /// <param name="result"></param>
    public delegate void DataReaderByRef<TKey, TState, TData, TResult>(TKey key, TState state, TData data, ref TResult result) where TData : class where TKey : notnull;
}
