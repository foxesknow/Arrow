using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Collections.Concurrent
{
    internal sealed class BucketConcurrentHashSet<T> : IConcurrentHashSet<T> where T : notnull
    {
        private readonly IConcurrentHashSet<T>[] m_Buckets;
        private readonly int m_NumberOfBuckets;

        private readonly IEqualityComparer<T> m_Comparer;

        internal BucketConcurrentHashSet(IEqualityComparer<T> comparer, IConcurrentHashSet<T>[] buckets)
        {
            if(buckets is null) throw new ArgumentNullException(nameof(buckets));

            m_Buckets = buckets;
            m_NumberOfBuckets = buckets.Length;
            m_Comparer = comparer;
        }

        /// <inheritdoc/>
        public bool Add(T value)
        {
            return Bucket(value).Add(value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            for(int i = 0; i < m_NumberOfBuckets; i++)
            {
                m_Buckets[i].Clear();
            }
        }

        /// <inheritdoc/>
        public bool Contains(T value)
        {
            return Bucket(value).Contains(value);
        }

        /// <inheritdoc/>
        public bool HasData()
        {
            for(int i = 0; i < m_NumberOfBuckets; i++)
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
        public bool Remove(T value)
        {
            return Bucket(value).Remove(value);
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < m_NumberOfBuckets; i++)
            {
                foreach(var value in m_Buckets[i])
                {
                    yield return value;
                }
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IConcurrentHashSet<T> Bucket(T key)
        {
            if(key is null) throw new ArgumentNullException(nameof(key));

            var hash = m_Comparer.GetHashCode(key) & int.MaxValue;
            var bucket = hash % m_NumberOfBuckets;

            return m_Buckets[bucket];
        }
    }

    /// <summary>
    /// Factory methods for a bucket concurrent sets
    /// </summary>
    public static class BucketConcurrentHashSet
    {
        /// <summary>
        /// Creates a bucket concurrent set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numberOfBuckets"></param>
        /// <returns></returns>
        public static IConcurrentHashSet<T> Make<T>(int numberOfBuckets) where T : notnull
        {
            return Make<T>(numberOfBuckets, null);
        }

        /// <summary>
        /// Creates a bucket concurrent set
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="numberOfBuckets"></param>
        /// <param name="comparer"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IConcurrentHashSet<T> Make<T>(int numberOfBuckets, IEqualityComparer<T>? comparer) where T : notnull
        {
            if(numberOfBuckets < 1) throw new ArgumentException("need at least one bucket", nameof(numberOfBuckets));

            // Nice and easy..!
            if(numberOfBuckets == 1)
            {
                return new ConcurrentHashSet<T>(comparer);
            }

            if(comparer is null) comparer = EqualityComparer<T>.Default;

            var buckets = new IConcurrentHashSet<T>[numberOfBuckets];
            for(var i = 0; i < numberOfBuckets; i++)
            {
                buckets[i] = new ConcurrentHashSet<T>(comparer);
            }

            return new BucketConcurrentHashSet<T>(comparer, buckets);
        }
    }
}
