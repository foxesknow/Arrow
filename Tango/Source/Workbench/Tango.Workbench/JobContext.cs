﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tango.Workbench
{
    /// <summary>
    /// Job context information.
    /// This class and its implementations are thread safe.
    /// </summary>
    public abstract class JobContext
    {
        private long m_ScopeID = 1;

        private AsyncLocal<long> m_AsyncScopeID = new();

        protected JobContext()
        {
            m_AsyncScopeID.Value = m_ScopeID;
        }

        internal long EnterNewAsyncScope()
        {
            var id = Interlocked.Increment(ref m_ScopeID);
            m_AsyncScopeID.Value = id;

            return id;
        }

        /// <summary>
        /// Creates a command for the specified database
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public IDbCommand CreateCommand(string databaseName)
        {
            var scopeID = m_AsyncScopeID.Value;
            return CreateCommand(scopeID, databaseName);
        }

        protected abstract IDbCommand CreateCommand(long scopeID, string databaseName);

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
