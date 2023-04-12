using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// A delegate used to write data to an alertable data manager
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TState"></typeparam>
    /// <typeparam name="TData"></typeparam>
    /// <param name="key"></param>
    /// <param name="state"></param>
    /// <param name="currentValue"></param>
    /// <returns></returns>
    public delegate TData? DataWriter<TKey, TState, TData>(TKey key, TState state, TData? currentValue) where TData : class where TKey : notnull;
}
