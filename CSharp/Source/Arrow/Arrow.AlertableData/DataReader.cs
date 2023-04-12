using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// A delegate used to read data from an alertable data manager
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="key"></param>
    /// <param name="state"></param>
    /// <param name="data"></param>
    /// <returns></returns>
    public delegate TResult DataReader<TKey, TState, TData, TResult>(TKey key, TState state, TData data) where TData : class where TKey : notnull;
}
