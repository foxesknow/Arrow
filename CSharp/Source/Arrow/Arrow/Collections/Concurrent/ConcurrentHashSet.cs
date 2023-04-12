using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Collections.Concurrent
{
    /// <summary>
    /// A concurrent hashset
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ConcurrentHashSet<T> : IConcurrentHashSet<T> where T : notnull
    {
        private readonly OptimizedConcurrentDictionary<T, byte> m_Values;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public ConcurrentHashSet() : this(null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public ConcurrentHashSet(IEqualityComparer<T>? comparer)
        {
            m_Values = new(comparer);
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public ConcurrentHashSet(int concurrencyLevel) : this(concurrencyLevel, null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public ConcurrentHashSet(int concurrencyLevel, IEqualityComparer<T>? comparer)
        {
            m_Values = new(concurrencyLevel, 1, comparer);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Add(T value)
        {
            return m_Values.TryAdd(value, 1);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            m_Values.Clear();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T value)
        {
            return m_Values.ContainsKey(value);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            return m_Values.Keys().GetEnumerator();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasData()
        {
            return m_Values.HasData();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty()
        {
            return m_Values.IsEmpty();
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T value)
        {
            return m_Values.TryRemove(value, out var _);
        }

        /// <inheritdoc/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
