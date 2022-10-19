using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Execution
{
    public static class DisposableExtensions
    {
        /// <summary>
        /// Combines 2 disposables into 1.
        /// The head will be dispose first, followed by the tail
        /// </summary>
        /// <param name="head"></param>
        /// <param name="tail"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDisposable Cons(this IDisposable head, IDisposable tail)
        {
            if(head is null) throw new ArgumentNullException(nameof(head));
            if(tail is null) throw new ArgumentNullException(nameof(tail));

            return new DisposableNode(head, tail);
        }

        private sealed class DisposableNode : IDisposable
        {
            private IDisposable? m_Head;
            private IDisposable? m_Tail;

            public DisposableNode(IDisposable head, IDisposable tail)
            {
                m_Head = head;
                m_Tail = tail;
            }

            public void Dispose()
            {
                if(m_Head is not null)
                {
                    m_Head.Dispose();
                    m_Head = null;
                }

                if(m_Tail is not null)
                {
                    m_Tail.Dispose();
                    m_Tail = null;
                }
            }
        }
    }
}
