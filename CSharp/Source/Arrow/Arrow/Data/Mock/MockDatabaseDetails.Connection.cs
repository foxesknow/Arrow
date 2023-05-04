using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data.Mock
{
    public sealed partial class MockDatabaseDetails
    {
        private sealed class Connection : DbConnection
        {
            private readonly MockDatabaseDetails m_Outer;

            private ConnectionState m_ConnectionState;
            private string m_DatabaseName = "";

            public Connection(MockDatabaseDetails outer, string connectionString)
            {
                m_Outer = outer;
                this.ConnectionString = connectionString;
            }

            [AllowNull]
            public override string ConnectionString{get; set;}

            public override string Database
            {
                get{return m_DatabaseName;}
            }

            public override string DataSource{get;} = "";

            public override string ServerVersion{get;} = "";

            public override ConnectionState State
            {
                get{return m_ConnectionState;}
            }

            public override void ChangeDatabase(string databaseName)
            {
                m_DatabaseName = databaseName;
            }

            public override void Close()
            {
                m_ConnectionState = ConnectionState.Closed;
            }

            public override void Open()
            {
                m_ConnectionState = ConnectionState.Open;
            }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            {
                if(this.IsOpen) return new Transaction(this, isolationLevel);

                throw new DataException("connection is not open");
            }

            protected override DbCommand CreateDbCommand()
            {
                if(this.IsOpen) return new Command(m_Outer, this);

                throw new DataException("connection is not open");
            }

            private bool IsOpen
            {
                get{return (m_ConnectionState & ConnectionState.Open) != 0;}
            }
        }
    }
}
