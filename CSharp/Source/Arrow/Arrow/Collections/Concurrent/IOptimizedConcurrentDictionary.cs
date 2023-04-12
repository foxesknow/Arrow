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
    public interface IOptimizedConcurrentDictionary<TKey, TValue> : IReadOnlyOptimizedConcurrentDictionary<TKey, TValue> where TKey : notnull
    {
        /// <summary>
        /// Gets or sets a value for a key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public new TValue this[TKey key]{get; set;}

        /// <summary>
        /// Add or updates a value 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="addValue"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValue, Func<TKey, TValue, TValue> updateValue);

        /// <summary>
        /// Adds or updates a value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="updateValue"></param>
        /// <returns></returns>
        public TValue AddOrUpdate(TKey key, TValue value, Func<TKey, TValue, TValue> updateValue);

        /// <summary>
        /// Adds or replaces a value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddOrReplace(TKey key, TValue value);

        /// <summary>
        /// Clears the dictionary
        /// </summary>
        public void Clear();

        /// <summary>
        /// Gets or adds an item to the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="makeValue"></param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> makeValue);

        /// <summary>
        /// Gets or adds a value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public TValue GetOrAdd(TKey key, TValue value);

        /// <summary>
        /// Tries to add a value to the dictionary if it does not already exist
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryAdd(TKey key, TValue value);

        /// <summary>
        /// Attempts to remove an item
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value);

        /// <summary>
        /// Attempts to update an item in the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="newValue"></param>
        /// <param name="comparisonValue"></param>
        /// <returns></returns>
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue);
    }
}
