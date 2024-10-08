﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

using Arrow.Text;
using Arrow.Execution;

using Tango.Workbench.Data;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Writes the incoming data to a csv file.
    /// 
    /// The sturcture of the first item will dictate the column headers and what values are written to the file.
    /// </summary>
    [Filter("ToCsv")]
    public sealed class ToCsvFilter : FileFilterBase
    {
        protected override async IAsyncEnumerable<object> WriteToFile(string filename, IAsyncEnumerable<object> items)
        {
            using(var stream = File.Create(filename, 16384))
            {
                RegisterRollbackFile(filename);

                using(var writer = new StreamWriter(stream))
                {
                    IReadOnlyList<string>? columns = null;

                    await foreach(var item in items)
                    {
                        var structuredObject = ToStructuredObject(item);

                        // If it's the first item then we'll use it as the column definition
                        if(columns is null) 
                        {
                            columns = structuredObject.Select(pair => pair.Key).ToList();
                            await writer.WriteAsync(string.Join(",", columns));
                        }

                        var line = string.Join(",", columns.Select(c => PropertyToCsv(c, structuredObject)));
                        
                        // Writing it like this means we don't get an empty line at the end of the file
                        await writer.WriteLineAsync();
                        await writer.WriteAsync(line);

                        yield return item;
                    }

                }
            }
        }

        private string PropertyToCsv(string property, IReadOnlyStructuredObject structuredObject)
        {
            structuredObject.TryGetValue(property, out var value);
            return ValueToString(value);
        }
        
        private string ValueToString(object? value)
        {
            if(value is null) return "";

            return value switch
            {
                string s    => Csv.Escape(s),
                DateTime d  => Csv.Escape(d.ToString("o", CultureInfo.InvariantCulture)),
                var other   => Csv.Escape(other.ToString() ?? "")
            };
        }

        public string DateTimeFormat{get; set;} = "o";
    }
}
