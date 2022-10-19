using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

#nullable enable

namespace Arrow.Data
{
    public interface IDatabaseManager
    {
        public IDbConnection OpenConnection(string databaseName);
        
        public IDbConnection OpenDynamicConnection(string databaseName, IReadOnlyDictionary<string, object> arguments);

        public IEnumerable<string> DatabaseNames{get;}

        public ConnectionInfo GetConnectionInfo(string databaseName);
    }

    public static class IDatabaseManagerExtensions
    {
        public static bool IsTransactional(this IDatabaseManager manager, string databaseName)
        {
            if(manager is null) throw new ArgumentNullException(nameof(manager));

            var info = manager.GetConnectionInfo(databaseName);
            return (info & ConnectionInfo.Transactional) != 0;
        }

        public static bool IsDynamic(this IDatabaseManager manager, string databaseName)
        {
            if(manager is null) throw new ArgumentNullException(nameof(manager));

            var info = manager.GetConnectionInfo(databaseName);
            return (info & ConnectionInfo.Dynamic) != 0;
        }

        public static bool HasDatabase(this IDatabaseManager manager, string databaseName)
        {
            if(manager is null) throw new ArgumentNullException(nameof(manager));
            if(databaseName is null) throw new ArgumentNullException(nameof(databaseName));
            if(string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("invalid database name", nameof(databaseName));

            return manager.DatabaseNames.Any(name => string.Compare(name, databaseName, true) == 0);
        }
    }
}
