using Arrow.Threading.Tasks;
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

        /// <summary>
        /// A disposer that does nothing
        /// </summary>
        public static readonly IAsyncDisposable NullAsync = new NullAsyncDisposable();

        /// <summary>
        /// Attempts to call eiter DisposeAsync or Dispose on an object if it
        /// implements the appropriate interface
        /// </summary>
        /// <param name="object"></param>
        /// <returns>true if one of the dispose methods was called, otherwise false</returns>
        public static ValueTask<bool> TryDisposeAsync(object? @object)
        {
            if(@object is IAsyncDisposable asyncDisposable)
            {
                // This way we'll only allocate the async/await state machine
                // if it's an IAsyncDisposable. For all other cases there's no allocation.
                return ExecuteDisposeAsync(asyncDisposable);
            }
            else if(@object is IDisposable disposable)
            {
                disposable.Dispose();
                return new(true);
            }

            return new(false);

            static async ValueTask<bool> ExecuteDisposeAsync(IAsyncDisposable asyncDisposable)
            {            
                await asyncDisposable.DisposeAsync().ContinueOnAnyContext();
                return true;
            }
        }

        private sealed class NullDisposable : IDisposable
        {
            public void Dispose()
            {
            }
        }

        private sealed class NullAsyncDisposable : IAsyncDisposable
        {
            public ValueTask DisposeAsync()
            {
                return default;
            }
        }
    }
}
