using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Collections.Concurrent
{
    /// <summary>
    /// An optimized dictionary that combines multipe dictionaries into one.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    internal sealed class BucketOptimizedConcurrentDictionary<TKey, TValue> : IOptimizedConcurrentDictionary<TKey, TValue> where TKey : notnull
    {
        private readonly IOptimizedConcurrentDictionary<TKey, TValue>[] m_Buckets;
        private readonly int m_NumberOfBuckets;

        private readonly IEqualityComparer<TKey> m_Comparer;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="buckets"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal BucketOptimizedConcurrentDictionary(IEqualityComparer<TKey> comparer, IOptimizedConcurrentDictionary<TKey, TValue>[] buckets)
        {
            if(buckets is null) throw new ArgumentNullException(nameof(buckets));

            m_Comparer = comparer;
            m_Buckets = buckets;
            m_NumberOfBuckets = buckets.Length;
        }

        /// <inheritdoc/>
        public TValue this[TKey key] 
        { 
            get{return Bucket(key)[key];}
            set{Bucket(key)[key] = value;}
        }

        /// <inheritdoc/>
        TValue IReadOnlyOptimizedConcurrentDictionary<TKey, TValue>.this[TKey key]
        {
            get{return Bucket(key)[key];}
        }

        /// <inheritdoc/>
        public void AddOrReplace(TKey key, TValue value)
        {
            Bucket(key).AddOrReplace(key, value);
        }

        /// <inheritdoc/>
        public TValue AddOrUpdate(TKey key, Func<TKey, TValue> addValue, Func<TKey, TValue, TValue> updateValue)
        {
            return Bucket(key).AddOrUpdate(key, addValue, updateValue);
        }

        /// <inheritdoc/>
        public TValue AddOrUpdate(TKey key, TValue value, Func<TKey, TValue, TValue> updateValue)
        {
            return Bucket(key).AddOrUpdate(key, value, updateValue);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            for(var i = 0; i < m_NumberOfBuckets; i++)
            {
                m_Buckets[i].Clear();
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return Bucket(key).ContainsKey(key);
        }

        /// <inheritdoc/>
        public TValue GetOrAdd(TKey key, Func<TKey, TValue> makeValue)
        {
            return Bucket(key).GetOrAdd(key, makeValue);
        }

        /// <inheritdoc/>
        public TValue GetOrAdd(TKey key, TValue value)
        {
            return Bucket(key).GetOrAdd(key, value);
        }

        /// <inheritdoc/>
        public bool HasData()
        {
            for(var i = 0; i < m_NumberOfBuckets; i++)
            {
                if(m_Buckets[i].HasData()) return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public bool IsEmpty()
        {
            return HasData() == false;
        }

        /// <inheritdoc/>
        public bool TryAdd(TKey key, TValue value)
        {
            return Bucket(key).TryAdd(key, value);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return Bucket(key).TryGetValue(key, out value);
        }

        /// <inheritdoc/>
        public bool TryRemove(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            return Bucket(key).TryRemove(key, out value);
        }

        /// <inheritdoc/>
        public bool TryUpdate(TKey key, TValue newValue, TValue comparisonValue)
        {
            return Bucket(key).TryUpdate(key, newValue, comparisonValue);
        }

        /// <inheritdoc/>
        public IEnumerable<TKey> Keys()
        {
            return m_Buckets.SelectMany(bucket => bucket.Keys());
        }

        /// <inheritdoc/>
        public IEnumerable<TValue> Values()
        {
            return m_Buckets.SelectMany(bucket => bucket.Values());
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for(var i = 0; i < m_NumberOfBuckets; i++)
            {
               foreach(var pair in m_Buckets[i])
               {
                    yield return pair;
               }
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IOptimizedConcurrentDictionary<TKey, TValue> Bucket(TKey key)
        {
            if(key is null) throw new ArgumentNullException(nameof(key));

            var hash = m_Comparer.GetHashCode(key) & int.MaxValue;
            var bucket = hash % m_NumberOfBuckets;

            return m_Buckets[bucket];
        }
    }


    /// <summary>
    /// Factory methods for a bucket dictionary
    /// </summary>
    public static class BucketOptimizedConcurrentDictionary
    {
        /// <summary>
        /// Creates a bucket dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="numberOfBuckets"></param>
        /// <returns></returns>
        public static IOptimizedConcurrentDictionary<TKey, TValue> Make<TKey, TValue>(int numberOfBuckets) where TKey : notnull
        {
            return Make<TKey, TValue>(numberOfBuckets, null);
        }

        /// <summary>
        /// Creates a bucket dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="numberOfBuckets"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IOptimizedConcurrentDictionary<TKey, TValue> Make<TKey, TValue>(int numberOfBuckets, IEqualityComparer<TKey>? comparer) where TKey : notnull
        {
            if(numberOfBuckets < 1) throw new ArgumentException("need at least one bucket", nameof(numberOfBuckets));

            // Nice and easy..!
            if(numberOfBuckets == 1)
            {
                return new OptimizedConcurrentDictionary<TKey, TValue>(comparer);
            }

            if(comparer is null) comparer = EqualityComparer<TKey>.Default;

            var buckets = new IOptimizedConcurrentDictionary<TKey, TValue>[numberOfBuckets];
            for(var i = 0; i < numberOfBuckets; i++)
            {
                buckets[i] = new OptimizedConcurrentDictionary<TKey, TValue>(comparer);
            }

            return new BucketOptimizedConcurrentDictionary<TKey, TValue>(comparer, buckets);
        }
    }
}
