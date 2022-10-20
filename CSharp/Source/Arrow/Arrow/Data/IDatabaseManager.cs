using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

#nullable enable

namespace Arrow.Data
{
    /// <summary>
    /// Defines the behaviour of a database manager
    /// </summary>
    public interface IDatabaseManager
    {
        /// <summary>
        /// Opens a connection to a database.
        /// The caller is responsible for closing the connection
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public IDbConnection OpenConnection(string databaseName);
        
        /// <summary>
        /// Opens a dynamic connection to a database.
        /// The caller is responsible for closing the connection
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public IDbConnection OpenDynamicConnection(string databaseName, IReadOnlyDictionary<string, object> arguments);

        /// <summary>
        /// Returns the names of all registered databases
        /// </summary>
        public IEnumerable<string> DatabaseNames{get;}

        /// <summary>
        /// Returns connection information about a database
        /// </summary>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        public ConnectionInfo GetConnectionInfo(string databaseName);
    }

    public static class IDatabaseManagerExtensions
    {
        /// <summary>
        /// Checks to see if a database is transactional
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsTransactional(this IDatabaseManager manager, string databaseName)
        {
            if(manager is null) throw new ArgumentNullException(nameof(manager));

            var info = manager.GetConnectionInfo(databaseName);
            return (info & ConnectionInfo.Transactional) != 0;
        }

        /// <summary>
        /// Checks to see if a database is dynamic
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool IsDynamic(this IDatabaseManager manager, string databaseName)
        {
            if(manager is null) throw new ArgumentNullException(nameof(manager));

            var info = manager.GetConnectionInfo(databaseName);
            return (info & ConnectionInfo.Dynamic) != 0;
        }

        /// <summary>
        /// Checks to see if a database exists
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="databaseName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static bool HasDatabase(this IDatabaseManager manager, string databaseName)
        {
            if(manager is null) throw new ArgumentNullException(nameof(manager));
            if(databaseName is null) throw new ArgumentNullException(nameof(databaseName));
            if(string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("invalid database name", nameof(databaseName));

            return manager.DatabaseNames.Any(name => string.Compare(name, databaseName, true) == 0);
        }

        /// <summary>
        /// returns the names of all non dynamic databases
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IEnumerable<string> NonDynamicDatabaseName(this IDatabaseManager manager)
        {
            if(manager is null) throw new ArgumentNullException(nameof(manager));

            return manager.DatabaseNames.Where(name => manager.IsDynamic(name) == false);
        }
    }
}
