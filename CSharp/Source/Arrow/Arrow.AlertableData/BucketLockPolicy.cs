using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// Converts any lock policy into a bucketed implementation.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class BucketLockPolicy<TKey> : ILockPolicy<TKey> where TKey : notnull
    {
        private readonly IDataLock[] m_Locks;
        private readonly int m_NumberOfLocks;

        private readonly ILockPolicy<TKey> m_LockPolicy;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="numberOfLocks"></param>
        /// <exception cref="ArgumentException"></exception>
        public BucketLockPolicy(int numberOfLocks, ILockPolicy<TKey> lockPolicy)
        {
            if(numberOfLocks < 1) throw new ArgumentException("need at least one lock", nameof(numberOfLocks));
            if(lockPolicy is null) throw new ArgumentNullException(nameof(lockPolicy));

            m_NumberOfLocks = numberOfLocks;
            m_Locks = new IDataLock[numberOfLocks];
            m_LockPolicy = lockPolicy;
        }

        public IDataLock Allocate(IEqualityComparer<TKey> comparer, TKey key)
        {
            var hash = comparer.GetHashCode(key) & int.MaxValue;
            var bucket = hash % m_NumberOfLocks;

            // It it's already there then easy.
            // Over time the whole array will be populated and this will be the default case.
            var dataLock = m_Locks[bucket];
            if(dataLock is not null) return dataLock;

            dataLock = m_LockPolicy.Allocate(comparer, key);

            var existing = Interlocked.CompareExchange(ref m_Locks[bucket], dataLock, null);
            if(existing is null)
            {
                return dataLock;
            }

            // There was already a lock in the slot.
            // This can happen in a multithreaded environment where there wasn't one when we
            // initially checked and then another thread came along and set it!
            
            // We don't need the instance we allocated
            m_LockPolicy.Reclaim(comparer, key, dataLock);

            return existing;
        }

        public void Reclaim(IEqualityComparer<TKey> comparer, TKey key, IDataLock dataLock)
        {
            
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"Bucket({m_LockPolicy})";
        }
    }
}
