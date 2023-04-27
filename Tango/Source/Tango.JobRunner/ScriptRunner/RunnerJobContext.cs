using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;
using Arrow.Execution;
using Arrow.Logging;

using Tango.JobRunner;

namespace ScriptRunner
{
    internal class RunnerJobContext : JobContext
    {
        private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), "[Context]");

        private readonly Dictionary<string, IDbConnection> m_Connections = new(StringComparer.OrdinalIgnoreCase);
        
        private readonly Dictionary<string, IDbConnection> m_TransactedConnections = new(StringComparer.OrdinalIgnoreCase);        
        private readonly Dictionary<string, IDbTransaction> m_Transactions = new(StringComparer.OrdinalIgnoreCase);

        private readonly object m_SyncRoot = new();

        private readonly RunnerRunSheet m_Script;
        private readonly IDatabaseManager m_DatabaseManager;

        public RunnerJobContext(RunnerRunSheet script)
        {
            m_Script = script;
            m_DatabaseManager = m_Script.DatabaseManager;
        }

        /// <summary>
        /// Indicates if transactions should be used.
        /// </summary>
        public bool UseTransactions{get; set;}

        /// <summary>
        /// Creates a command against the specified database.
        /// If the database is already open then the command will come from that connection
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        /// <exception cref="JobRunnerException"></exception>
        public override IDbCommand MakeCommand(string databaseName)
        {
            lock(m_SyncRoot)
            {
                var connectionInfo = m_DatabaseManager.GetConnectionInfo(databaseName);
                return (connectionInfo, UseTransactions) switch
                {
                    (ConnectionInfo.Transactional, true)    => MakeTransactionalCommand(databaseName),
                    (ConnectionInfo.Transactional, false)   => MakeNonTransactionalCommand(databaseName),
                    (ConnectionInfo.Default, _)             => MakeNonTransactionalCommand(databaseName),
                    var other                               => throw new JobRunnerException("unsupported connection type: {other}")
                };
            }
        }

        protected override void Commit()
        {
            lock(m_SyncRoot)
            {
                var exceptions = FinalizeTransactions("commit", static transaction => transaction.Commit());
                ResetDatabases();
                
                if(exceptions.Count != 0) throw new AggregateException(exceptions);
            }
        }

        protected override void Rollback()
        {
            lock(m_SyncRoot)
            {
                var exceptions = FinalizeTransactions("rollback", static transaction => transaction.Rollback());
                ResetDatabases();
                
                if(exceptions.Count != 0) throw new AggregateException(exceptions);
            }
        }

        private List<Exception> FinalizeTransactions(string action, Action<IDbTransaction> finalizer)
        {
            var exceptions = new List<Exception>();

            foreach(var (database, transaction) in m_Transactions)
            {
                try
                {
                    finalizer(transaction);
                }
                catch(Exception e)
                {
                    Log.Error($"Could not {action} on {database}", e);
                    exceptions.Add(e);
                }
            }

            return exceptions;
        }

        private IDbCommand MakeNonTransactionalCommand(string databaseName)
        {
            if(m_Connections.TryGetValue(databaseName, out var connection) == false)
            {
                connection = m_DatabaseManager.OpenConnection(databaseName);
                m_Connections.Add(databaseName, connection);
            }

            return connection.CreateCommand();
        }

        private IDbCommand MakeTransactionalCommand(string databaseName)
        {
            if(m_TransactedConnections.TryGetValue(databaseName, out var connection) == false)
            {
                connection = m_DatabaseManager.OpenConnection(databaseName);                
                m_TransactedConnections.Add(databaseName, connection);

                var transaction = connection.BeginTransaction();
                m_Transactions.Add(databaseName, transaction);
            }

            var command = connection.CreateCommand();
            command.Transaction = m_Transactions[databaseName];

            return command;
        }

        private void ResetDatabases()
        {
            foreach(var (_, connection) in m_Connections)
            {
                MethodCall.AllowFail(connection, static c => c.Close());
            }

            foreach(var (_, connection) in m_TransactedConnections)
            {
                MethodCall.AllowFail(connection, static c => c.Close());
            }

            m_Connections.Clear();
            m_TransactedConnections.Clear();
            m_Transactions.Clear();
        }
    }
}
