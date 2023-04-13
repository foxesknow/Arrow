using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// A lock policy based on the .NET monitor.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public sealed class MonitorLockPolicy<TKey> : ILockPolicy<TKey> where TKey : notnull
    {
        public IDataLock Allocate(IEqualityComparer<TKey> comparer, TKey key)
        {
            return new MonitorLock();
        }

        public void Reclaim(IEqualityComparer<TKey> comparer, TKey key, IDataLock dataLock)
        {
            // No need to do anything
        }

        private sealed class MonitorLock : IDataLock
        {
            // NOTE: We'll use "this" to avoid needless garbage.
            // Kids, don't do this at home :-)

            public void EnterRead(ref bool lockTaken)
            {
                Monitor.Enter(this, ref lockTaken);
            }

            public void ExitRead(bool lockTaken)
            {
                if(lockTaken) Monitor.Exit(this);
            }

            public void EnterWrite(ref bool lockTaken)
            {
                Monitor.Enter(this, ref lockTaken);
            }

            public void ExitWrite(bool lockTaken)
            {
                if(lockTaken) Monitor.Exit(this);
            }
        }
    }
}
