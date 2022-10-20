using Arrow.Configuration;
using Arrow.Xml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

#nullable enable

namespace Arrow.Data.DatabaseManagers
{
    /// <summary>
    /// A database manager that is populated from the "connectionStrings" section of app.config
    /// </summary>
    public sealed class AppConfigDatabaseManager : DatabaseManagerBase, IDatabaseManager
    {
        private readonly Dictionary<string, (Database Database, ConnectionInfo ConnectionInfo)> m_ConnectionInfo = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Creates a database instance from an app.config "connectionStrings" section
        /// </summary>
        /// <param name="connectionStringSettings"></param>
        /// <returns></returns>
        public delegate (Database Database, ConnectionInfo ConnectionInfo) DatabaseFactory(ConnectionStringSettings connectionStringSettings);

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="databaseFactory"></param>
        public AppConfigDatabaseManager(DatabaseFactory databaseFactory) : this(databaseFactory, null)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="databaseFactory"></param>
        /// <param name="isAllowed">Decides which connections will be used. If null then all connections will be allowed</param>
        /// <exception cref="ArgumentNullException"></exception>
        public AppConfigDatabaseManager(DatabaseFactory databaseFactory, Func<ConnectionStringSettings, bool>? isAllowed)
        {
            if(databaseFactory is null) throw new ArgumentNullException(nameof(databaseFactory));

            if(isAllowed is null)
            {
                isAllowed = static _ => true;
            }

            InitializeFromAppConfig(databaseFactory, isAllowed);
        }

        IEnumerable<string> IDatabaseManager.DatabaseNames
        {
            get{return m_ConnectionInfo.Keys;}
        }

        ConnectionInfo IDatabaseManager.GetConnectionInfo(string databaseName)
        {
            ValidateDatabaseName(databaseName);

            if(m_ConnectionInfo.TryGetValue(databaseName, out var details))
            {
                return details.ConnectionInfo;
            }

            throw new DataException($"database not found: {databaseName}");
        }


        IDbConnection IDatabaseManager.OpenConnection(string databaseName)
        {
            ValidateDatabaseName(databaseName);

            if(m_ConnectionInfo.TryGetValue(databaseName, out var d))
            {
                return DoOpenConnection(databaseName, d.Database.Details);
            }

            throw new DataException($"database not found: {databaseName}");
        }

        IDbConnection IDatabaseManager.OpenDynamicConnection(string databaseName, IReadOnlyDictionary<string, object> arguments)
        {
            throw new NotImplementedException("dynamic databases are not supported");
        }

        private void InitializeFromAppConfig(DatabaseFactory databaseFactory, Func<ConnectionStringSettings, bool> isAllowed)
        {
            var document = AppConfig.ConfigDocument;
            if(document is null) return;

            var nodes = document.DocumentElement!.SelectSingleNode("connectionStrings");
            if(nodes is null) return;

            foreach(XmlNode? node in nodes.SelectNodesOrEmpty("*"))
            {
                if(node is null) continue;

                if(node.LocalName != "add") continue;

                var name = node.Attributes!.GetValueOrDefault("name", null);
                
                if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid database name");
                if(m_ConnectionInfo.ContainsKey(name!)) throw new DataException($"duplicate argument name: {name}");

                var connectionString = node.Attributes!.GetValueOrDefault("connectionString", "");
                var providerName = node.Attributes!.GetValueOrDefault("providerName", "");

                var connectionSettings = new ConnectionStringSettings()
                {
                    Name = name,
                    ProviderName = providerName,
                    ConnectionString = connectionString
                };

                if(isAllowed(connectionSettings))
                {
                    var details = databaseFactory(connectionSettings);
                    if(details.Database is null) throw new DataException($"factory did not return a database for {name}");

                    var transational = (details.ConnectionInfo  & ConnectionInfo.Transactional) != 0;
                    if(transational != details.Database.Transactional) throw new DataException($"conflicting transactional status for {name}");

                    m_ConnectionInfo.Add(name!, details);
                }
            }
        }
    }
}
