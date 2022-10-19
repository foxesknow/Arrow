using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Logging
{
    /// <summary>
    /// Flags a log implementation as supporting a property context
    /// </summary>
    public interface IPropertyContext
    {
        /// <summary>
        /// Gets the pusher for a log, of null if not supported
        /// </summary>
        /// <returns></returns>
        public IPropertyPusher? GetPusher();
    }
}
