using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.IO
{
    /// <summary>
    /// Specified how the DirectoryExpander will behave during expansion
    /// </summary>
    public enum DirectoryExpanderMode
    {
        /// <summary>
        /// Only returns directories that actually exist
        /// </summary>
        OnlyExisting,

        /// <summary>
        /// Returns everything
        /// </summary>
        Everything
    }
}
