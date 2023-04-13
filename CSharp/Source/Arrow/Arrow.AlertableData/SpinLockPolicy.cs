using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// A lock policy based on spin locks
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class SpinLockPolicy<TKey> : ILockPolicy<TKey> where TKey : notnull
    {
        /// <inheritdoc/>
        public IDataLock Allocate(IEqualityComparer<TKey> comparer, TKey key)
        {
            return new SpinDataLock();
        }

        /// <inheritdoc/>
        public void Reclaim(IEqualityComparer<TKey> comparer, TKey key, IDataLock dataLock)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "SpinLock";
        }

        private sealed class SpinDataLock : IDataLock
        {
            // NOTE: DO NOT make this read-only!
            private SpinLock m_SpinLock = new();

            public void EnterRead(ref bool lockTaken)
            {
                m_SpinLock.Enter(ref lockTaken);
            }

            public void ExitRead(bool lockTaken)
            {
                if(lockTaken) m_SpinLock.Exit();
            }

            public void EnterWrite(ref bool lockTaken)
            {
                m_SpinLock.Enter(ref lockTaken);
            }

            public void ExitWrite(bool lockTaken)
            {
                if(lockTaken) m_SpinLock.Exit();
            }
        }
    }
}
