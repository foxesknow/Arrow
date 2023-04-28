using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Collections
{
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
    public static class AsyncEnumerableExtensions
    {
        /// <summary>
        /// Converts an IEnumerable sequence to an IAsyncEnumerable sequence
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
        {
            if(source is null) throw new ArgumentNullException(nameof(source));

            return new AsyncEnumerableWrapper<T>(source);
        }

        private class AsyncEnumerableWrapper<T> : IAsyncEnumerable<T>
        {
            private readonly IEnumerable<T> m_Source;

            public AsyncEnumerableWrapper(IEnumerable<T> source)
            {
                m_Source = source;
            }

            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
            {
                return new AsyncEnumeratorWrapper(m_Source.GetEnumerator());
            }

            private class AsyncEnumeratorWrapper : IAsyncEnumerator<T>
            {
                private IEnumerator<T>? m_Source;

                public AsyncEnumeratorWrapper(IEnumerator<T> source)
                {
                    m_Source = source;
                }

                T IAsyncEnumerator<T>.Current
                {
                    get{return m_Source!.Current;}
                }

                ValueTask IAsyncDisposable.DisposeAsync()
                {
                    if(m_Source is not null)
                    {
                        m_Source.Dispose();
                        m_Source = null;
                    }

                    return default;
                }

                ValueTask<bool> IAsyncEnumerator<T>.MoveNextAsync()
                {
                    return new(m_Source!.MoveNext());
                }
            }
        }
    }
#endif
}
