using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

#nullable enable

namespace Arrow.Data.DatabaseManagers
{
    public abstract class DatabaseManagerBase
    {
        protected virtual IDbConnection DoOpenConnection(string name, DatabaseDetails? databaseDetails)
        {
            if(databaseDetails is null) throw new InvalidOperationException($"database details not configured for {name}");
        
            var connection = databaseDetails.CreateConnection();
            connection.Open();

            return connection;
        }
    }
}
