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
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source, CancellationToken cancellationToken = default)
        {
            if(source is null) throw new ArgumentNullException(nameof(source));

            return new AsyncEnumerableWrapper<T>(source, cancellationToken);
        }

        private class AsyncEnumerableWrapper<T> : IAsyncEnumerable<T>
        {
            private readonly IEnumerable<T> m_Source;
            private readonly CancellationToken m_CancellationToken;

            public AsyncEnumerableWrapper(IEnumerable<T> source, CancellationToken cancellationToken)
            {
                m_Source = source;
                m_CancellationToken = cancellationToken;
            }

            IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
            {
                return new AsyncEnumeratorWrapper(m_Source.GetEnumerator(), m_CancellationToken);
            }

            private class AsyncEnumeratorWrapper : IAsyncEnumerator<T>
            {
                private IEnumerator<T>? m_Source;
                private readonly CancellationToken m_CancellationToken;

                public AsyncEnumeratorWrapper(IEnumerator<T> source, CancellationToken cancellationToken)
                {
                    m_Source = source;
                    m_CancellationToken = cancellationToken;
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
                    m_CancellationToken.ThrowIfCancellationRequested();
                    return new(m_Source!.MoveNext());
                }
            }
        }
    }
#endif
}
