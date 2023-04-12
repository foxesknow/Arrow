using System;
using System.Data;

using Arrow.Data;

using Oracle.ManagedDataAccess.Client;

namespace Arrow.Oracle
{
    /// <summary>
    /// Returns the database details for an Oracle database
    /// </summary>
    public sealed class OracleDatabaseDetails : DatabaseDetails
    {
        /// <summary>
        /// The oracle connection string
        /// </summary>
        public string? ConnectionString{get; set;}

        /// <inheritdoc/>
        public override IDbConnection CreateConnection()
        {
            if(this.ConnectionString == null) throw new ArgumentNullException(nameof(ConnectionString));

            var connection = new OracleConnection(this.ConnectionString);
            return connection;
        }
    }
}