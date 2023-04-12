using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    public abstract class ReadWriteLockPolicyBase
    {
        protected class Lock : IDataLock, IDisposable
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
