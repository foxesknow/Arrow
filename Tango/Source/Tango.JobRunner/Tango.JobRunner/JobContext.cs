using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    /// <summary>
    /// Job context information.
    /// This class and it's implementations are thread safe.
    /// </summary>
    public abstract class JobContext
    {
        /// <summary>
        /// Creates a command for the specified database
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public abstract IDbCommand MakeCommand(string databaseName);

        /// <summary>
        /// Commits any transactions
        /// </summary>
        protected internal abstract void Commit();
        
        /// <summary>
        /// Rolls back any transactions
        /// </summary>
        protected internal abstract void Rollback();
    }
}
