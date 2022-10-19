using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data.DatabaseManagers
{
    /// <summary>
    /// The transaction mode
    /// </summary>
    public enum TransactionMode
    {
        /// <summary>
        /// The connection is non transactional
        /// </summary>
        NonTransactional,

        /// <summary>
        /// The connection is transactional
        /// </summary>
        Transactional
    }
}
