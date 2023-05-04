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
        private sealed class Parameter : DbParameter
        {
            private string? m_ParameterName;
            private string? m_SourceColumn;

            public Parameter()
            {
                this.DbType = DbType.String;
                this.Direction = ParameterDirection.Input;
                this.ParameterName = "";
                this.SourceColumn = "";
            }

            public override DbType DbType{get; set;}
            public override ParameterDirection Direction{get; set;}
            public override bool IsNullable{get; set;}

            [AllowNull]
            public override string ParameterName
            {
                get{return m_ParameterName ?? "";}
                set{m_ParameterName = value;}
            }

            [AllowNull]
            public override string SourceColumn
            {
                get{return m_SourceColumn ?? "";}
                set{m_SourceColumn = value;}
            }

            public override int Size{get; set;}
            public override bool SourceColumnNullMapping{get; set;}
            public override object? Value{get; set;}

            public override void ResetDbType()
            {
                this.DbType = DbType.String;
            }
        }
    }
}
