using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Logging
{
    /// <summary>
    /// Implemented by loggers that can attach context to a log
    /// </summary>
    public interface IForContext
    {
        /// <summary>
        /// Returns a logger with context for the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public ILog ForContext<T>();

        /// <summary>
        /// Returns a logger with context for the specified type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILog ForContext(Type? type);
        
        /// <summary>
        /// Returns a loggerwith the spcified context
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ILog ForContext(string? name);
    }
}
