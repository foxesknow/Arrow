using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// Allocates one ReaderWriterLockSlim per key.
    /// NOTE: The underlying ReaderWriterLockSlim is not reenterant.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class ReadWriteLockPolicy<TKey> : ILockPolicy<TKey> where TKey : notnull
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        public ReadWriteLockPolicy()
        {
        }

        /// <inheritdoc/>>
        public IDataLock Allocate(IEqualityComparer<TKey> comparer, TKey key)
        {
            return new Lock();
        }

        /// <inheritdoc/>>
        public void Reclaim(IEqualityComparer<TKey> comparer, TKey key, IDataLock dataLock)
        {
            if(dataLock is IDisposable d)
            {
                d.Dispose();
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return "ReadWriteLock";
        }

        private sealed class Lock : IDataLock, IDisposable
        {
            private readonly ReaderWriterLockSlim m_Lock = new();
            private bool m_Disposed;

            public void Dispose()
            {
                if(m_Disposed == false)
                {
                    m_Lock.Dispose();
                    m_Disposed = true;
                }
            }

            public void EnterRead(ref bool lockTaken)
            {
                m_Lock.EnterReadLock();
                lockTaken = true;
            }

            public void ExitRead(bool lockTaken)
            {
                if(lockTaken) m_Lock.ExitReadLock();
            }

            public void EnterWrite(ref bool lockTaken)
            {
                m_Lock.EnterWriteLock();
                lockTaken = true;
            }

            public void ExitWrite(bool lockTaken)
            {
                if(lockTaken) m_Lock.ExitWriteLock();
            }
        }
    }
}
