using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data.Mock
{
    public sealed partial class MockDatabaseDetails
    {
        private sealed class Command : DbCommand
        {
            private readonly MockDatabaseDetails m_Details;

            public Command(MockDatabaseDetails details, DbConnection connection)
            {
                m_Details = details;

                this.DbConnection = connection;
                this.CommandType = CommandType.Text;
                this.CommandText = "";
                this.UpdatedRowSource = UpdateRowSource.Both;
            }

            [AllowNull]
            public override string CommandText{get; set;}
    
            public override int CommandTimeout{get; set;} = 15;
            public override CommandType CommandType{get; set;}
            public override bool DesignTimeVisible{get; set;}
            public override UpdateRowSource UpdatedRowSource{get; set;}
            protected override DbConnection? DbConnection{get; set;}

            protected override DbParameterCollection DbParameterCollection{get;} = new ParameterCollection();

            protected override DbTransaction? DbTransaction{get; set;}

            public override void Cancel()
            {
                // Do nothing
            }

            public override int ExecuteNonQuery()
            {
                return m_Details.ExecuteNonQuery(this);
            }

            public override object? ExecuteScalar()
            {
                return m_Details.ExecuteScalar(this);
            }

            public override void Prepare()
            {
                // Do nothing
            }

            protected override DbParameter CreateDbParameter()
            {
                return new Parameter();
            }

            protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            {
                return m_Details.ExecuteReader(this);
            }
        }
    }
}
