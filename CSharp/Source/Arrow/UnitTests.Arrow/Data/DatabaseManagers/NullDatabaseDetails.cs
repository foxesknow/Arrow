using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;

namespace UnitTests.Arrow.Data.DatabaseManagers
{
    public class NullDatabaseDetails : DatabaseDetails
    {
        public override IDbConnection CreateConnection()
        {
            IDbConnection c = new NullConnection();
            c.ConnectionString = this.ConnectionString;

            return c;
        }

        public string ConnectionString{get; set;}
    }

    class NullConnection : IDbConnection
    {
        private ConnectionState m_State;

        string IDbConnection.ConnectionString{get; set;}

        int IDbConnection.ConnectionTimeout => throw new NotImplementedException();

        string IDbConnection.Database => throw new NotImplementedException();

        ConnectionState IDbConnection.State
        {
            get{return m_State;}
        }

        IDbTransaction IDbConnection.BeginTransaction()
        {
            throw new NotImplementedException();
        }

        IDbTransaction IDbConnection.BeginTransaction(IsolationLevel il)
        {
            throw new NotImplementedException();
        }

        void IDbConnection.ChangeDatabase(string databaseName)
        {
            throw new NotImplementedException();
        }

        void IDbConnection.Close()
        {
            throw new NotImplementedException();
        }

        IDbCommand IDbConnection.CreateCommand()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
        }

        void IDbConnection.Open()
        {
            m_State = ConnectionState.Open;
        }
    }
}
