using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Threading.Tasks
{
    public static partial class TaskEx
    {
        
        /// <summary>
        /// Allows code to switch from its current synchronization context to another one.
        /// </summary>
        /// <example>
        ///     await TaskEx.SwitchToContect(targetContext);
        ///     // For here on you're running on targetContext
        /// </example>
        /// <param name="context">The context to swith to. If null then after an await code will resume in the thread pool</param>
        /// <returns></returns>
        public static Impl.SwitchToContextAwaiter SwitchToContext(SynchronizationContext? context)
        {
            return new(context);
        }

        public static class Impl
        {
            public struct SwitchToContextAwaiter : INotifyCompletion
            {
                private readonly SynchronizationContext? m_TargetContext;

                public SwitchToContextAwaiter(SynchronizationContext? targetContext)
                {
                    m_TargetContext = targetContext;
                }

                public bool IsCompleted
                {
                    get{return m_TargetContext == SynchronizationContext.Current;}
                }

                public void GetResult()
                {
                    // Does nothing
                }

                public void OnCompleted(Action continuation)
                {
                    if(m_TargetContext is null)
                    {
                        // As there's no target we'll run on the thread pool
                        ThreadPool.QueueUserWorkItem(static state => ((Action)state!)(), continuation);
                    }
                    else
                    {
                        m_TargetContext.Post(static state => ((Action)state!)(), continuation);
                    }
                }

                /// <summary>
                /// Returns the awaitable instance, which is just a copy of this struct
                /// </summary>
                /// <returns></returns>
                public SwitchToContextAwaiter GetAwaiter()
                {
                    return this;
                }
            }
        }
    }
}
