using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data
{
    [Flags]
    public enum ConnectionInfo
    {
        /// <summary>
        /// The default mode (non-transactional)
        /// </summary>
        Default = 0,

        /// <summary>
        /// The connection starts a transaction
        /// </summary>
        Transactional = 1,

        /// <summary>
        /// The connection supports dynamic arguments
        /// </summary>
        Dynamic = 2
    }
}
