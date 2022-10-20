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

            public override void Post(SendOrPostCallback d, object? state)
            {
                try
                {
                    m_Queue.ContextEnqueue(d, state);
                }
                catch(Exception e)
                {
                    if(m_Queue.RaiseUnhandledThreadException(e)) throw;
                }
            }

            public override void Send(SendOrPostCallback continuation, object? state)
            {
                if(m_Queue.ID == AsyncWorkQueue.ActiveID)
                {
                    // We're already on the queue, so just call the callback
                    try
                    {
                        continuation(state);
                    }
                    catch(Exception e)
                    {
                        if(m_Queue.RaiseUnhandledThreadException(e)) throw;
                    }
                }
                else
                {
                    // We need to marshall onto our queue and wait for the call to finish
                    var task = DoSend(continuation, state);
                    
                    // Waiting this way will unpack any exception that is thrown
                    task.GetAwaiter().GetResult();
                }
            }

            private Task DoSend(SendOrPostCallback continuation, object? state)
            {
                try
                {
                    return m_Queue.EnqueueAsync((continuation, state), s =>
                    {
                        s.continuation(s.state);
                    });
                }
                catch(Exception e)
                {
                    if(m_Queue.RaiseUnhandledThreadException(e)) throw;

                    return Task.CompletedTask;
                }
            }
        }
    }
}
