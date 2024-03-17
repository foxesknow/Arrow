using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters.ScriptSupport
{
    /// <summary>
    /// Useful functions to inject to expose via the filter scripts
    /// </summary>
    public static class Wire
    {
        /// <summary>
        /// Throws an exception
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="WorkbenchException"></exception>
        public static object? Throw(string message)
        {
            throw new WorkbenchException(message);
        }
    }
}
