using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// Allocates one lock per key
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class ReadWriteLockPolicy<TKey> : ReadWriteLockPolicyBase, ILockPolicy<TKey> where TKey : notnull
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
    }
}
