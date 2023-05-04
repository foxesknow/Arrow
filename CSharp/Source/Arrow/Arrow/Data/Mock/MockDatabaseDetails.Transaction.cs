using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data.Mock
{
    public sealed partial class MockDatabaseDetails
    {
        private sealed class Transaction : DbTransaction
        {
            public Transaction(Connection connection, IsolationLevel isolationLevel)
            {
                this.DbConnection = connection;
                this.IsolationLevel = isolationLevel;
            }

            public override IsolationLevel IsolationLevel{get;}

            protected override DbConnection? DbConnection{get;}

            public override void Commit()
            {
            }

            public override void Rollback()
            {
            }
        }
    }
}
