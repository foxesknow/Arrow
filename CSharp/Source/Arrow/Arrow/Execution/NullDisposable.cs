using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Execution
{
    /// <summary>
    /// An implementation of IDisposable that does nothing
    /// </summary>
    public sealed class NullDisposable : IDisposable
    {
        /// <summary>
        /// A shareable instance of the class
        /// </summary>
        public static readonly IDisposable Instance = new NullDisposable();

        void IDisposable.Dispose()
        {
            // Does nothing
        }
    }
}
