using System;
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

        private readonly CancellationTokenSource m_Cts = new();

        protected JobContext()
        {
            m_AsyncScopeID.Value = m_ScopeID;
            this.CancellationToken = m_Cts.Token;
        }

        public CancellationToken CancellationToken{get;}

        /// <summary>
        /// Called by the implementation to indicate that it is starting a new logic scope
        /// and that the framework should make any necessary changes that may be required.
        /// 
        /// For example, database connections cannot be shared across threads. When we enter
        /// a new scope we will create a new set of connections for them,
        /// </summary>
        /// <returns></returns>
        internal long EnterNewAsyncScope()
        {
            var id = Interlocked.Increment(ref m_ScopeID);
            m_AsyncScopeID.Value = id;

            return id;
        }

        /// <summary>
        /// Signals the cancellation token
        /// </summary>
        internal void Cancel()
        {
            m_Cts.Cancel();
        }

        /// <summary>
        /// Schedules a cancellation after a given delay
        /// </summary>
        /// <param name="delay"></param>
        internal void CancelAfter(TimeSpan delay)
        {
            m_Cts.CancelAfter(delay);
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

        /// <summary>
        /// Disposes of any resources.
        /// NOTE: We don't do this via IDisposable as we don't want to expose the interface to consumers.
        /// </summary>
        protected internal virtual void Dispose()
        {
            m_Cts.Dispose();
        }
    }
}
