using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;

using Tango.Workbench.Data;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Uses paramaterized sql to interact with a database
    /// </summary>
    [Filter("ParameterizedSql")]
    public sealed class ParameterizedSqlFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Database is null) throw new WorkbenchException("no database specified");
            if(this.Sql is null) throw new WorkbenchException("no sql specified");

            var expander = MakeExpander();

            await foreach(var item in items)
            {
                var structuredObject = ToStructuredObject(item);

                using(var command = this.Context.CreateCommand(this.Database))
                using(var disposer = new ParameterDisposer())
                {
                    command.CommandText = expander.Expand(this.Sql);

                    foreach(var info in this.Parameters)
                    {
                        var dbType = info.DBType;
                        var name = info.Parameter!;
                        var value = GetValue(structuredObject, info);

                        var parameter = command.CreateParameter(dbType, name, value);
                        command.Parameters.Add(parameter);
                        disposer.Add(parameter);
                    }

                    var rowsAffected = command.ExecuteNonQuery();
                    VerboseLog.Info($"{rowsAffected} row(s) affected");
                }

                yield return item;

            }
        }

        private StructuredObject ToStructuredObject(object obj)
        {
            if(obj is StructuredObject structuredObject) return structuredObject;

            return StructuredObject.From(obj);
        }

        private object GetValue(StructuredObject @object, ParameterInfo parameterInfo)
        {
            var value = @object[parameterInfo.Property!];
            if(value is null) return DBNull.Value;
            
            object mappedValue = parameterInfo.DBType switch
            {
                DbType.String       => Convert.ToString(value)!,
                DbType.Boolean      => Convert.ToBoolean(value),
                DbType.Byte         => Convert.ToByte(value),
                DbType.SByte        => Convert.ToSByte(value),
                DbType.Int16        => Convert.ToInt16(value),
                DbType.UInt16       => Convert.ToUInt16(value),
                DbType.Int32        => Convert.ToInt32(value),
                DbType.UInt32       => Convert.ToUInt32(value),
                DbType.Int64        => Convert.ToInt64(value),
                DbType.UInt64       => Convert.ToUInt64(value),
                DbType.Single       => Convert.ToSingle(value),
                DbType.Double       => Convert.ToDouble(value),
                DbType.Decimal      => Convert.ToDecimal(value),
                DbType.DateTime     => Convert.ToDateTime(value),
                DbType.DateTime2    => Convert.ToDateTime(value),
                DbType.Guid         => (value is Guid g ? g : Guid.Parse(value.ToString()!)),
                _                   => throw new WorkbenchException($"unsupported DbType: {parameterInfo.DBType}")
            };

            return mappedValue;
        }

        /// <summary>
        /// The database to connect to
        /// </summary>
        public string? Database{get; set;}

        /// <summary>
        /// The sql to run
        /// </summary>
        public string? Sql{get; set;}

        /// <summary>
        /// The parameters to the sql
        /// </summary>
        public List<ParameterInfo> Parameters{get;} = new();


        public class ParameterInfo
        {
            /// <summary>
            /// The name of the parameter to set
            /// </summary>
            public string? Parameter{get; set;}
            
            /// <summary>
            /// The database type of the parameter to set
            /// </summary>
            public DbType DBType{get; set;}

            /// <summary>
            /// The property to use on the incoming object
            /// </summary>
            public string? Property{get; set;}
        }
    }
}
