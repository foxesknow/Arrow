using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// Defines the lock policy used by alertable data producers and consumers.
    /// Locks are not required to be reentrant.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface ILockPolicy<TKey> where TKey : notnull
    {
        /// <summary>
        /// Allocates a lock
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public IDataLock Allocate(IEqualityComparer<TKey> comparer, TKey key);

        /// <summary>
        /// Gives the lock back to the policy as the caller has decided it is not required
        /// </summary>
        /// <param name="comparer"></param>
        /// <param name="key"></param>
        /// <param name="dataLock"></param>
        public void Reclaim(IEqualityComparer<TKey> comparer, TKey key, IDataLock dataLock);
    }
}
