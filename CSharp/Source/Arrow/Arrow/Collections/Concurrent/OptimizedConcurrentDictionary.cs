using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Collections.Concurrent
{
    public sealed class OptimizedConcurrentDictionary<TKey, TValue> : IOptimizedConcurrentDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly ConcurrentDictionary<TKey, TValue> m_Data;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public OptimizedConcurrentDictionary()
        {
            m_Data = new();
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="comparer"></param>
        public OptimizedConcurrentDictionary(IEqualityComparer<TKey>? comparer)
        {
            if(comparer is null)
            {
                m_Data = new();
            }
            else
            {
                m_Data = new(comparer);
            }
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="concurrencyLevel"></param>
        /// <param name="capacity"></param>
        public OptimizedConcurrentDictionary(int concurrencyLevel, int capacity) : this(concurrencyLevel, capacity, null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="concurrencyLevel"></param>
        /// <param name="capacity"></param>
        /// <param name="comparer"></param>
        public OptimizedConcurrentDictionary(int concurrencyLevel, int capacity, IEqualityComparer<TKey>? comparer)
        {
            if(comparer is null)
            {
                m_Data = new(concurrencyLevel, capacity);
            }
            else
            {
                m_Data = new(concurrencyLevel, capacity, comparer);
            }
        }

        /// <inheritdoc/>
        public TValue this[TKey key] 
        { 
            get{return m_Data[key];}
            set{m_Data[key] = value;}
        }

        /// <inheritdoc/>
        TValue IReadOnlyOptimizedConcurrentDictionary<TKey, TValue>.this[TKey key] 
        {
            get{return m_Data[key];}
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddOrReplace(TKey key, TValue value)
        {
            m_Data[key] = value;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValue, Func<TKey, TValue, TValue> updateValue)
        {
            return m_Data.AddOrUpdate(key, addValue, updateValue);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue AddOrUpdate(TKey key, TValue value, Func<TKey, TValue, TValue> updateValue)
        {
            return m_Data.AddOrUpdate(key, value, updateValue);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            m_Data.Clear();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsKey(TKey key)
        {
            return m_Data.ContainsKey(key);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> makeValue)
        {
            return m_Data.GetOrAdd(key, makeValue);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TValue GetOrAdd(TKey key, TValue value)
        {
            return m_Data.GetOrAdd(key, value);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasData()
        {
            // We Skip(0) as Linq has an optimization for ICollection.Count
            // that will lock the entire dictionary
            return m_Data.Skip(0).Any();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty()
        {
            return HasData() == false;
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TKey> Keys()
        {
            return m_Data.Select(pair => pair.Key);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryAdd(TKey key, TValue value)
        {
            return m_Data.TryAdd(key, value);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return m_Data.TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return m_Data.TryRemove(key, out value);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            return m_Data.TryUpdate(key, newValue, comparisonValue);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<TValue> Values()
        {
            return m_Data.Select(pair => pair.Value);
        }
    }
}
