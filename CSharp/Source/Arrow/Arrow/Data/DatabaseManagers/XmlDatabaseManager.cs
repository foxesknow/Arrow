using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Xml;

using Arrow.Xml;
using Arrow.Xml.ObjectCreation;

#nullable enable

namespace Arrow.Data.DatabaseManagers
{
    public sealed class XmlDatabaseManager : DatabaseManagerBase, IDatabaseManager, ISupportInitialize
    {
        private readonly Dictionary<string, ConnectionInfo> m_ConnectionInfo = new(StringComparer.OrdinalIgnoreCase);

        public XmlDatabaseManager()
        {

        }

        /// <summary>
        /// A list of registered databases
        /// </summary>
        public List<Database> Databases{get;} = new();
        
        /// <summary>
        /// Dynamic databases that are created on demand using parameters supplied at runtime
        /// </summary>
        public XmlNode? DynamicDatabases{get; set;}

        /// <inheritdoc/>
        IEnumerable<string> IDatabaseManager.DatabaseNames
        {
            get{return m_ConnectionInfo.Keys;}
        }

        /// <inheritdoc/>
        ConnectionInfo IDatabaseManager.GetConnectionInfo(string databaseName)
        {
            ValidateDatabaseName(databaseName);

            if(m_ConnectionInfo.TryGetValue(databaseName, out var connectionInfo))
            {
                return connectionInfo;
            }

            throw new DataException($"database not found: {databaseName}");
        }

        /// <inheritdoc/>
        IDbConnection IDatabaseManager.OpenConnection(string databaseName)
        {
            ValidateDatabaseName(databaseName);

            IDatabaseManager self = this;
            if(self.IsDynamic(databaseName)) throw new DataException($"database is dynamic: {databaseName}");

            var database = GetDatabase(databaseName);
            return DoOpenConnection(databaseName, database.Details);
        }

        /// <inheritdoc/>
        IDbConnection IDatabaseManager.OpenDynamicConnection(string databaseName, IReadOnlyDictionary<string, object> arguments)
        {
            ValidateDatabaseName(databaseName);
            if(arguments is null) throw new ArgumentNullException(nameof(arguments));

            IDatabaseManager self = this;
            if(self.IsDynamic(databaseName) == false) throw new DataException($"database is not dynamic: {databaseName}");

            var database = MakeDynamicDatabase(databaseName, arguments);
            return DoOpenConnection(databaseName, database.Details);
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
            foreach(var database in this.Databases)
            {
                ValidateDatabaseName(database.Name);
                if(database.Details is null) throw new ArgumentException($"no database details specified for {database.Name}");

                if(m_ConnectionInfo.ContainsKey(database.Name!))
                {
                    throw new DataException($"a database with this name is already registered: {database.Name}");
                }

                var connectionInfo = ConnectionInfo.Default;
                if(database.Transactional) connectionInfo |= ConnectionInfo.Transactional;

                m_ConnectionInfo.Add(database.Name!, connectionInfo);
            }

            if(this.DynamicDatabases is not null)
            {
                foreach(XmlNode? node in this.DynamicDatabases.SelectNodeOrEmpty("*"))
                {
                    if(node is null) continue;

                    var name = node.Attributes!.GetValueOrDefault("name", null);
                    if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid dynamic database name");

                    if(m_ConnectionInfo.ContainsKey(name!))
                    {
                        throw new DataException($"a database with this name is already registered: {name}");
                    }

                    var connectionInfo = ConnectionInfo.Dynamic;
                    var transactional = node.Attributes!.GetValueOrDefault("transactional", "false");
                    if(bool.TryParse(transactional, out var isTransactional))
                    {
                        if(isTransactional) connectionInfo |= ConnectionInfo.Transactional;
                    }
                    else
                    {
                        throw new DataException($"invalid transactional value for dynamic database: {name}");
                    }

                    m_ConnectionInfo.Add(name!, connectionInfo);
                }
            }
        }

        private Database GetDatabase(string databaseName)
        {
            var database = this.Databases.FirstOrDefault(d => string.Compare(databaseName, d.Name, true) == 0);

            if(database is null) throw new DataException($"could not find database: {databaseName}");
            if(database.Details is null) throw new DataException($"no details for database: {databaseName}");

            return database;
        }

        private Database MakeDynamicDatabase(string databaseName, IReadOnlyDictionary<string, object> arguments)
        {
            // We want case insensitive arguments
            var normalizedArguments = NormalizeArguments(arguments);
            var node = GetDynamicDatabase(databaseName);

            var factory = InstanceFactory.New().UnknownVariableLookup(name =>
            {
                normalizedArguments.TryGetValue(name, out var value);
                return value;
            });

            var database = factory.Create<Database>(node);
            return database;
        }

        private IReadOnlyDictionary<string, object> NormalizeArguments(IReadOnlyDictionary<string, object> arguments)
        {
            var normalized = new Dictionary<string, object>();

            foreach(var pair in arguments)
            {
                var lowerKey = pair.Key.ToLower();
                if(normalized.ContainsKey(lowerKey)) throw new DataException($"duplicate argument name: {pair.Key}");

                normalized.Add(lowerKey, pair.Value);
            }

            return normalized;
        }

        private XmlNode GetDynamicDatabase(string databaseName)
        {
            if(this.DynamicDatabases is null) throw new DataException($"could not find database: {databaseName}");

            foreach(XmlNode? node in this.DynamicDatabases.SelectNodeOrEmpty("*"))
            {
                if(node is null) continue;

                var name = node.Attributes!.GetValueOrDefault("name", null);
                if(name is null) continue;

                if(string.Compare(name, databaseName, true) == 0) return node;
            }

            throw new DataException($"could not find dynamic database: {databaseName}");
        }
    }
}
