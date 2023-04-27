using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Jobs
{
    [Job("ExecuteSql")]
    public sealed class ExecuteSqlJob : Job
    {
        public override ValueTask Run()
        {
            if(this.Database is null) throw new JobRunnerException("no database specified");
            if(this.Sql is null) throw new JobRunnerException("no sql specified");

            using(var command = this.Context.MakeCommand(this.Database))
            {
                command.CommandText = this.Sql;
                command.ExecuteNonQuery();
            }
        }

        public string? Database{get; set;}

        public string? Sql{get; set;}
    }
}
