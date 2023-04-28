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
    /// This class and its implementations are thread safe.
    /// </summary>
    public abstract class JobContext
    {
        /// <summary>
        /// Creates a command for the specified database
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public abstract IDbCommand CreateCommand(string databaseName);

        /// <summary>
        /// The directory where the currently executing script resides
        /// </summary>
        public abstract string ScriptDirectory{get;}

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
