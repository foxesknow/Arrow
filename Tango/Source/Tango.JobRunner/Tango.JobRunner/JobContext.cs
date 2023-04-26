using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner
{
    public abstract class JobContext
    {
        /// <summary>
        /// Creates a command for the specified database
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public abstract IDbCommand MakeCommand(string databaseName);

        public abstract void Commit();
        
        public abstract void Rollback();
    }
}
