using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.AlertableData
{
    /// <summary>
    /// A lock returned from a Read
    /// 
    /// NOTE: This lock is only for use in a using() statement.
    /// You must ensure that Dispose() is called ONLY once.
    /// </summary>
    public readonly ref struct ReadLock
    {
        private readonly bool m_LockTaken;
        private readonly IDataLock m_Lock;

        public ReadLock(bool lockTaken, IDataLock @lock)
        {
            m_LockTaken = lockTaken;
            m_Lock = @lock;
        }

        public void Dispose()
        {
            if(m_Lock is not null) m_Lock.ExitRead(m_LockTaken);
        }
    }
}
