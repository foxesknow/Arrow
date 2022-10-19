using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

#nullable enable

namespace Arrow.Data.DatabaseManagers
{
    public sealed class DatabaseManager : DatabaseManagerBase, IDatabaseManager
    {
        private readonly Dictionary<string, Details> m_Details = new(StringComparer.OrdinalIgnoreCase);        

        public void Add(string name, TransactionMode mode, DatabaseDetails databaseDetails)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrEmpty(name)) throw new ArgumentException("invalid name", nameof(name));
            if(databaseDetails is null) throw new ArgumentNullException(nameof(databaseDetails));

            if(m_Details.ContainsKey(name)) throw new ArgumentException($"database already exists: {name}");

            var details = new Details
            {
                ConnectionInfo = MapeTransactionMode(mode),
                DatabaseDetails = databaseDetails
            };

            m_Details.Add(name, details);
        }

        public void AddDynamic(string name, TransactionMode mode, Func<IReadOnlyDictionary<string, object>, DatabaseDetails> factory)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrEmpty(name)) throw new ArgumentException("invalid name", nameof(name));
            if(factory is null) throw new ArgumentNullException(nameof(factory));

            if(m_Details.ContainsKey(name)) throw new ArgumentException($"database already exists: {name}");

            var details = new Details
            {
                ConnectionInfo = MapeTransactionMode(mode) | ConnectionInfo.Dynamic,
                Factory = factory
            };

            m_Details.Add(name, details);
        }

        ConnectionInfo IDatabaseManager.GetConnectionInfo(string databaseName)
        {
            if(databaseName is null) throw new ArgumentNullException(nameof(databaseName));
            if(string.IsNullOrEmpty(databaseName)) throw new ArgumentException("invalid name", nameof(databaseName));

            if(m_Details.TryGetValue(databaseName, out var details)) return details.ConnectionInfo;

            throw new DataException($"database not found: {databaseName}");
        }

        IDbConnection IDatabaseManager.OpenConnection(string databaseName)
        {
            if(databaseName is null) throw new ArgumentNullException(nameof(databaseName));
            if(string.IsNullOrEmpty(databaseName)) throw new ArgumentException("invalid name", nameof(databaseName));

            IDatabaseManager self = this;
            if(self.IsDynamic(databaseName)) throw new DataException($"database is dynamic: {databaseName}");

            if(m_Details.TryGetValue(databaseName, out var details))
            {
                return DoOpenConnection(databaseName, details.DatabaseDetails);
            }

            throw new DataException($"database not found: {databaseName}");
        }

        IDbConnection IDatabaseManager.OpenDynamicConnection(string databaseName, IReadOnlyDictionary<string, object> arguments)
        {
            if(databaseName is null) throw new ArgumentNullException(nameof(databaseName));
            if(string.IsNullOrEmpty(databaseName)) throw new ArgumentException("invalid name", nameof(databaseName));
            if(arguments is null) throw new ArgumentNullException(nameof(arguments));

            IDatabaseManager self = this;
            if(self.IsDynamic(databaseName) == false) throw new DataException($"database is not dynamic: {databaseName}");

            if(m_Details.TryGetValue(databaseName, out var details))
            {
                var databaseDetails = details.Factory!(arguments);
                return DoOpenConnection(databaseName, databaseDetails);
            }

            throw new DataException($"database not found: {databaseName}");
        }

        IEnumerable<string> IDatabaseManager.DatabaseNames
        {
            get{return m_Details.Keys;}
        }

        private ConnectionInfo MapeTransactionMode(TransactionMode mode)
        {
            return mode == TransactionMode.NonTransactional ? ConnectionInfo.Default : ConnectionInfo.Transactional;
        }

        class Details
        {
            public ConnectionInfo ConnectionInfo{get; set;}
            public DatabaseDetails ? DatabaseDetails{get; set;}
            public Func<IReadOnlyDictionary<string, object>, DatabaseDetails>? Factory{get; set;}
        }
    }
}
