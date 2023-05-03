using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Arrow.Text;

using Tango.Workbench.Data;

namespace Tango.Workbench.Filters
{
    [Filter("ToCsv")]
    public sealed class ToCsvFilter : FileFilterBase
    {
        protected override async IAsyncEnumerable<object> WriteToFile(string filename, IAsyncEnumerable<object> items)
        {
            using(var stream = File.Create(filename, 16384))
            using(var writer = new StreamWriter(stream))
            {
                IReadOnlyList<string>? columns = null;

                await foreach(var item in items)
                {
                    var structuredObject = StructuredObject.From(item);
                    
                    // If it's the first item then we'll use it as the column definition
                    if(columns is null) 
                    {
                        columns = structuredObject.Select(pair => pair.Key).ToList();
                        await writer.WriteLineAsync(string.Join(",", columns));
                    }

                    var line = string.Join(",", columns.Select(c => PropertyToCsv(c, structuredObject)));
                    await writer.WriteLineAsync(line);

                    yield return item;
                }
            }
        }

        private string PropertyToCsv(string property, StructuredObject structuredObject)
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
                var other   => other.ToString() ?? ""
            };
        }
    }
}
