using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Threading.Tasks
{
    public class AsyncSemaphore : AsyncWaitHandle
    {
        private readonly Queue<TaskCompletionSource<bool>> m_Waiters = new Queue<TaskCompletionSource<bool>>();

        private int m_CurrentCount;

        public AsyncSemaphore(int initialCount)
        {
            if(initialCount <= 0) throw new ArgumentException("need at least a count of one", nameof(initialCount));

            m_CurrentCount = initialCount;
        }

        public override Task WaitAsync()
        {
            lock(m_Waiters)
            {
                if(m_CurrentCount > 0)
                {
                    // We've still got resources to allocate
                    m_CurrentCount--;
                    return Task.CompletedTask;
                }
                else
                {
                    // We're all out of resources, so the caller will have to wait
                    var waiter = MakeTcs();
                    m_Waiters.Enqueue(waiter);

                    return waiter.Task;
                }
            }
        }

        /// <summary>
        /// Attempts to acquire a resource
        /// </summary>
        /// <returns></returns>
        public bool TryAccquire()
        {
            lock(m_Waiters)
            {
                if(m_CurrentCount > 0)
                {
                    m_CurrentCount--;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool>? source = null;

            lock(m_Waiters)
            {
                if(m_Waiters.Count > 0)
                {
                    source = m_Waiters.Dequeue();
                }
                else
                {
                    m_CurrentCount++;
                }
            }

            if(source is not null)
            {
                ReleaseTcs(source);
            }
        }
    }
}
