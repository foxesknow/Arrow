﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tango.Workbench.Data;

namespace Tango.Workbench.Sources
{
    [Source("Select")]
    public sealed class SelectSource : Source
    {        
        public override IAsyncEnumerable<object> Run()
        {
            if(this.Database is null) throw new ArgumentNullException(nameof(Database));
            if(this.Query is null) throw new ArgumentNullException(nameof(Query));

            return Execute(this.Database, this.Query);

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

        public string? Database{get; set;}
        
        
        public string? Query{get; set;}

        public long PageSize{get; set;} = 1000;
    }
}
