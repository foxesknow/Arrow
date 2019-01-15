using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public partial class AsyncWorkQueue
    {
        class CustomSynchronizationContext : SynchronizationContext
        {
            private readonly AsyncWorkQueue m_Queue;

            public CustomSynchronizationContext(AsyncWorkQueue queue)
            {
                m_Queue = queue;
            }

            public override SynchronizationContext CreateCopy()
            {
                return new CustomSynchronizationContext(m_Queue);
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                m_Queue.ContextEnqueueAsync(d, state);
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                var activeQueue = (SynchronizationContext.Current as CustomSynchronizationContext)?.m_Queue;

                if(object.ReferenceEquals(m_Queue, activeQueue))
                {
                    // We're already on the queue, so just call the callback
                    d(state);
                }
                else
                {
                    // We need to marshall onto our queue and wait for the call to finish
                    var task = m_Queue.EnqueueAsync(() => d(state));
                    task.GetAwaiter().GetResult();
                }
            }
        }
    }
}
