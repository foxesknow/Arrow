using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public class AsyncSemaphore : AsyncWaitHandle
    {
        private readonly Queue<TaskCompletionSource<bool>> m_Waiters = new Queue<TaskCompletionSource<bool>>();

        private int m_Count;
        private int m_MaximumCount;

        public AsyncSemaphore(int count)
        {
            if(count <= 0) throw new ArgumentException("need at least a count of one", nameof(count));

            m_Count = count;
            m_MaximumCount = count;
        }

        public override Task WaitAsync()
        {
            lock(m_Waiters)
            {
                if(m_Count != 0)
                {
                    // We've still got resources to allocate
                    m_Count--;
                    return Completed;
                }
                else
                {
                    // We're all out of resources, so the caller will have to wait
                    var waiter = new TaskCompletionSource<bool>();
                    m_Waiters.Enqueue(waiter);

                    return waiter.Task;
                }
            }
        }

        public void Release()
        {
            TaskCompletionSource<bool> source = null;

            lock(m_Waiters)
            {
                if(m_Count == m_MaximumCount)
                {
                    throw new SemaphoreFullException();
                }

                if(m_Waiters.Count != 0)
                {
                    // There's someone waiting.
                    // Don't bother adding the resource back, just release the waiter and give the resource to them
                    source = m_Waiters.Dequeue();
                }
                else
                {
                    m_Count++;
                }
            }

            if(source != null)
            {
                SetTaskCompletionSouce(source);
            }
        }
    }
}
