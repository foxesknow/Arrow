using System;
using System.Data;
using Arrow.Data;

using Npgsql;

namespace Arrow.Postgres
{
    public sealed class PostgresDatabaseDetails : DatabaseDetails
    {
        /// <inheritdoc/>
        public override IDbConnection CreateConnection()
        {
            if(this.ConnectionString == null) throw new ArgumentNullException(nameof(ConnectionString));

            var connection = new NpgsqlConnection(this.ConnectionString);
            return connection;
        }

        /// <summary>
        /// The Postgres connection string
        /// </summary>
        public string? ConnectionString{get; set;}
    }
}