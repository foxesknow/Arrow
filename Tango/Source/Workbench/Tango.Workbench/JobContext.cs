using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Execution;

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
        private AsyncLocal<CancellationTokenSource> m_AsyncCancellationTokenSource = new();

        private readonly CancellationTokenSource m_RootCts = new();

        private Stack<Func<IReadOnlyList<Exception>>> m_CommitStack = new();
        private Stack<Func<IReadOnlyList<Exception>>> m_RollbackStack = new();

        protected JobContext()
        {
            m_AsyncScopeID.Value = m_ScopeID;
            m_AsyncCancellationTokenSource.Value = m_RootCts;
        }

        protected object SyncRoot{get;} = new();

        public CancellationToken CancellationToken
        {
            get{return m_AsyncCancellationTokenSource.Value!.Token;}
        }

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

            // We need a new cancellation token source that will only affect this scope.
            // However, we link it back to its parent so that if the parent scope is
            // cancelled then we'll also be cancelled
            var activeCts = m_AsyncCancellationTokenSource.Value!;            
            var scopedCts = CancellationTokenSource.CreateLinkedTokenSource(activeCts.Token);
            m_AsyncCancellationTokenSource.Value = scopedCts;

            return id;
        }

        internal void LeaveAsyncScope()
        {
            var activeConsCell = m_AsyncCancellationTokenSource.Value;
            if(activeConsCell is not null)
            {
                activeConsCell.Dispose();
            }
        }

        /// <summary>
        /// Registers a function that will be called when the group commits
        /// </summary>
        /// <param name="commit"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterCommit(Func<IReadOnlyList<Exception>> commit)
        {
            if(commit is null) throw new ArgumentNullException(nameof(commit));

            lock(this.SyncRoot)
            {
                m_CommitStack.Push(commit);
            }
        }

        /// <summary>
        /// Registers a function that will be called when the group rolls back
        /// </summary>
        /// <param name="rollback"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RegisterRollback(Func<IReadOnlyList<Exception>> rollback)
        {
            if(rollback is null) throw new ArgumentNullException(nameof(rollback));

            lock(this.SyncRoot)
            {
                m_RollbackStack.Push(rollback);
            }
        }

        /// <summary>
        /// Signals the cancellation token
        /// </summary>
        public void Cancel()
        {
            m_AsyncCancellationTokenSource.Value!.Cancel();
        }

        /// <summary>
        /// Schedules a cancellation after a given delay
        /// </summary>
        /// <param name="delay"></param>
        public void CancelAfter(TimeSpan delay)
        {
            m_AsyncCancellationTokenSource.Value!.CancelAfter(delay);
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
        protected internal void Commit()
        {
            var allExceptions = new List<Exception>();

            lock(this.SyncRoot)
            {
                while(m_CommitStack.TryPop(out var function))
                {
                    try
                    {
                        var exceptions = function();
                        allExceptions.AddRange(exceptions);
                    }
                    catch(Exception e)
                    {
                        allExceptions.Add(e);
                    }
                }
            }

            if(allExceptions.Count != 0) throw new AggregateException(allExceptions);
        }
        
        /// <summary>
        /// Rolls back any transactions
        /// </summary>
        protected internal void Rollback()
        {
            var allExceptions = new List<Exception>();

            lock(this.SyncRoot)
            {
                while(m_RollbackStack.TryPop(out var function))
                {
                    try
                    {
                        var exceptions = function();
                        allExceptions.AddRange(exceptions);
                    }
                    catch(Exception e)
                    {
                        allExceptions.Add(e);
                    }
                }
            }

            if(allExceptions.Count != 0) throw new AggregateException(allExceptions);
        }

        /// <summary>
        /// Disposes of any resources.
        /// NOTE: We don't do this via IDisposable as we don't want to expose the interface to consumers.
        /// </summary>
        protected internal virtual void Dispose()
        {
            m_RootCts.Dispose();
        }
    }
}
