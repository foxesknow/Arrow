using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    public static partial class Disposable
    {
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER

        /// <summary>
        /// Converts a synchronois disposer into an asynchronous disposer
        /// </summary>
        /// <param name="disposable"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IAsyncDisposable SyncToAsync(IDisposable disposable)
        {
            if(disposable is null) throw new ArgumentNullException(nameof(disposable));

            return new SyncToAsyncDisposer(disposable);
        }

        private sealed class SyncToAsyncDisposer : IAsyncDisposable
        {
            private IDisposable? m_Outer;

            public SyncToAsyncDisposer(IDisposable outer)
            {
                m_Outer = outer;
            }

            /// <inheritdoc/>
            public ValueTask DisposeAsync()
            {
                if(m_Outer is not null)
                {
                    m_Outer.Dispose();
                    m_Outer = null;
                }

                return default;
            }
        }

#endif
    }
}
