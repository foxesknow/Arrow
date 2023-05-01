using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Filters
{
    [Filter("ExecuteSql")]
    public sealed class ExecuteSqlFilter : Filter
    {
        public override async IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Database is null) throw new WorkbenchException("no database specified");
            if(this.Sql is null) throw new WorkbenchException("no sql specified");

            var expander = MakeExpander();

            await foreach(var item in items)
            {
                using(var command = this.Context.CreateCommand(this.Database))
                {
                    expander.Add("item", item);

                    command.CommandText = expander.Expand(this.Sql);
                    var rowsAffected = command.ExecuteNonQuery();
                    VerboseLog.Info($"{rowsAffected} row(s) affected");
                }

                yield return item;
            }
        }

        /// <summary>
        /// The database to connect to
        /// </summary>
        public string? Database{get; set;}

        /// <summary>
        /// The sql to run
        /// </summary>
        public string? Sql{get; set;}
    }
}
