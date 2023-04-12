using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// A read-write lock policy that has a limited number of locks
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class BucketReadWriteLockPolicy<TKey> : ReadWriteLockPolicyBase, ILockPolicy<TKey> where TKey : notnull
    {
        private readonly Lock[] m_Locks;
        private readonly int m_NumberOfLocks;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="numberOfLocks"></param>
        /// <exception cref="ArgumentException"></exception>
        public BucketReadWriteLockPolicy(int numberOfLocks)
        {
            if(numberOfLocks < 1) throw new ArgumentException("need at least one lock", nameof(numberOfLocks));

            m_NumberOfLocks = numberOfLocks;
            m_Locks = Enumerable.Range(1, numberOfLocks)
                                .Select(_ => new Lock())
                                .ToArray();
        }

        /// <inheritdoc/>
        public IDataLock Allocate(IEqualityComparer<TKey> comparer, TKey key)
        {
            var hash = comparer.GetHashCode(key) & int.MaxValue;
            var bucket = hash % m_NumberOfLocks;

            return m_Locks[bucket];
        }

        /// <inheritdoc/>
        public void Reclaim(IEqualityComparer<TKey> comparer, TKey key, IDataLock dataLock)
        {
            // As we've dishing these out from a bucket we don't want to dispose 
            // of them as they're being shared
        }
    }
}
