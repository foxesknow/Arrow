using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Logging
{
    /// <summary>
    /// Pushes properties into a log context
    /// </summary>
    public interface IPropertyPusher
    {
        /// <summary>
        /// Pushes any properties whose name is not null or whitespace.
        /// Properties are pused into the context from left to right
        /// </summary>
        /// <param name="properties"></param>
        /// <returns>A disposable instance that will pop the properties from right to left</returns>
        public IDisposable Push(IEnumerable<(string Name, object? Value)> properties);
    }
}
