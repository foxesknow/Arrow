using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Jobs
{
    /// <summary>
    /// Executes sql against a database
    /// </summary>
    [Job("ExecuteSql")]
    public sealed class ExecuteSqlJob : Job
    {
        public override ValueTask Run()
        {
            if(this.Database is null) throw new JobRunnerException("no database specified");
            if(this.Sql is null) throw new JobRunnerException("no sql specified");

            using(var command = this.Context.CreateCommand(this.Database))
            {
                command.CommandText = this.Sql;
                var rowsAffected = command.ExecuteNonQuery();
                VerboseLog.Info($"{rowsAffected} row(s) affected");
            }

            return default;
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
