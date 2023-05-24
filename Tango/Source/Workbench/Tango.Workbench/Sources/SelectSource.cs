using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango.Workbench.Data;

namespace Tango.Workbench.Sources
{
    /// <summary>
    /// Runs sql and yields the rows as a structured object
    /// </summary>
    [Source("Select")]
    public sealed class SelectSource : Source
    {        
        public override IAsyncEnumerable<object> Run()
        {
            if(this.Database is null) throw new ArgumentNullException(nameof(Database));
            if(this.Sql is null) throw new ArgumentNullException(nameof(Sql));

            return Execute(this.Database, this.Sql);

            async IAsyncEnumerable<object> Execute(string database, string query)
            {
                await ForceAsync();

                using(var command = this.Context.CreateCommand(database))
                {
                    command.CommandText = query;

                    using(var reader = command.ExecuteReader())
                    {
                        var fieldNames = Enumerable.Range(0, reader.FieldCount)
                                                   .Select(index => reader.GetName(index))
                                                   .ToArray();

                        long count = 0;
                        long pageSize = this.PageSize;

                        while(reader.Read())
                        {
                            var @object = MakeStructuredObject(reader, fieldNames);
                            yield return @object;
                            count++;

                            if(pageSize != 0 && (count % pageSize) == 0)
                            {
                                VerboseLog.Info($"read {count:n0} items");
                            }
                        }
                    }
                }
            }
        }

        private StructuredObject MakeStructuredObject(IDataReader reader, IReadOnlyList<string> fieldNames)
        {
            var structuredObject = new StructuredObject();

            for(int i = 0; i < fieldNames.Count; i++)
            {
                var name = fieldNames[i];
                
                var value = reader[i];
                if(Convert.IsDBNull(value)) value = null;

                structuredObject.Add(name, value);
            }

            return structuredObject;
        }

        /// <summary>
        /// The database to connect to
        /// </summary>
        public string? Database{get; set;}
        
        /// <summary>
        /// The sql query to run
        /// </summary>
        public string? Sql{get; set;}

        /// <summary>
        /// How many items to process before writing progress information to the verbose log
        /// </summary>
        public long PageSize{get; set;} = 1000;
    }
}
