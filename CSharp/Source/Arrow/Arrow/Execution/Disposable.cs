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
    public static partial class Disposable
    {
        /// <summary>
        /// A disposer that does nothing
        /// </summary>
        public static readonly IDisposable Null = new NullDisposable();

        private class NullDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        /// <summary>
        /// A disposer that does nothing
        /// </summary>
        public static readonly IAsyncDisposable NullAsync = new NullAsyncDisposable();

        private sealed class NullAsyncDisposable : IAsyncDisposable
        {
            public ValueTask DisposeAsync()
            {
                return default;
            }
        }
    }
}
