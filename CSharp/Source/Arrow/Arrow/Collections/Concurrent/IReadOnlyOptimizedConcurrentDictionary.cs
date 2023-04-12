using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Collections.Concurrent
{
    /// <summary>
    /// Defines an interface to a concurrent dictionary.
    /// Removes all the methods that lock the entire dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IReadOnlyOptimizedConcurrentDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull
    {
        /// <summary>
        /// Gets the value for a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TValue this[TKey key]{get;}

        /// <summary>
        /// Checks to see if a key is in the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey(TKey key);

        /// <summary>
        /// Attempts to get an item from the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);

        /// <summary>
        /// Returns true if there is data in the collection, otherwise false
        /// </summary>
        /// <returns></returns>
        public bool HasData();

        /// <summary>
        /// Returns true if the dictionary is empty, otherwise false
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty();

        /// <summary>
        /// Returns the values
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TKey> Keys();

        /// <summary>
        /// Returns the values
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TValue> Values();
    }
}
