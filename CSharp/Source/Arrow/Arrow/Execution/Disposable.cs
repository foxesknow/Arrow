using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// Useful disposable stuff
    /// </summary>
    public static class Disposable
    {
        /// <summary>
        /// A disposer that does nothing
        /// </summary>
        public static readonly IDisposable Null = new NullDisposable();

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        /// <summary>
        /// A disposer that does nothing
        /// </summary>
        public static readonly IAsyncDisposable NullAsync = new NullAsyncDisposable();
#endif

        private class NullDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
        private class NullAsyncDisposable : IAsyncDisposable
        {
            public ValueTask DisposeAsync()
            {
                return default;
            }
        }
#endif
    }
}
