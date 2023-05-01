using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;

using Arrow.Execution;

using Tango.Workbench.Data;

namespace Tango.Workbench.Filters
{
    [Filter("ToJson")]
    public sealed class ToJsonFilter : Filter
    {
        public override IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Filename is null) throw new ArgumentNullException(nameof(Filename));

            var filename = MakeExpander().Expand(this.Filename);

            if(File.Exists(filename))
            {
                if(this.SkipExisting)
                {
                    Log.Info($"Skipping {filename}");
                    return items;
                }

                if(this.Overwrite)
                {
                    Log.Info($"deleting {filename}");
                    File.Delete(filename);
                }
                else
                {
                    throw new WorkbenchException($"zip file already exists: {filename}");
                }
            }

            return Execute(filename, items);

            async IAsyncEnumerable<object> Execute(string filename, IAsyncEnumerable<object> items)
            {
                var options = new JsonWriterOptions()
                {
                    Indented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                var succeeded = false;

                try
                {
                    using(var stream = File.Create(filename, 16384))
                    using(var writer = new Utf8JsonWriter(stream, options))
                    {
                        writer.WriteStartArray();

                        await foreach(var item in items)
                        {
                            WriteObject(writer, item);
                            yield return item;
                        }

                        writer.WriteEndArray();
                    }

                    succeeded = true;
                }
                finally
                {
                    if(succeeded == false)
                    {
                        Log.Error($"error whilst writing {filename}");
                        MethodCall.AllowFail(filename, static filename => File.Delete(filename));
                    }
                }
            }
        }

        private void WriteObject(Utf8JsonWriter writer, object @object)
        {
            var structuredObject = @object switch
            {
                StructuredObject s  => s,
                var other           => StructuredObject.From(other)
            };

            writer.WriteStartObject();

            foreach(var (name, value) in structuredObject)
            {
                writer.WritePropertyName(name);
                WriteValue(writer, value);
            }

            writer.WriteEndObject();
        }

        private void WriteValue(Utf8JsonWriter writer, object? value)
        {
            if(value is null)
            {
                writer.WriteNullValue();
                return;
            }

            switch(value)
            {
                case Enum e:
                    writer.WriteStringValue(e.ToString());
                    break;

                case bool b:
                    writer.WriteBooleanValue(b);
                    break;

                case char c:
                    writer.WriteStringValue(c.ToString());
                    break;

                case string s:
                    writer.WriteStringValue(s);
                    break;

                case byte number:
                    writer.WriteNumberValue(number);
                    break;

                case short number:
                    writer.WriteNumberValue(number);
                    break;

                case int number:
                    writer.WriteNumberValue(number);
                    break;

                case long number:
                    writer.WriteNumberValue(number);
                    break;

                case float number:
                    writer.WriteNumberValue(number);
                    break;

                case double number:
                    writer.WriteNumberValue(number);
                    break;

                case decimal number:
                    writer.WriteNumberValue(number);
                    break;

                case Guid guid:
                    writer.WriteStringValue(guid.ToString("D"));
                    break;

                case DateTime dateTime:
                    writer.WriteStringValue(dateTime.ToString("o", CultureInfo.InvariantCulture));
                    break;

                case DateTimeOffset dateTimeOffset:
                    writer.WriteStringValue(dateTimeOffset.ToString("o", CultureInfo.InvariantCulture));
                    break;

                case TimeSpan timeSpan:
                    writer.WriteStringValue(timeSpan.ToString());
                    break;

                case StructuredObject structuredObject:
                    WriteObject(writer, structuredObject);
                    break;

                case IEnumerable<KeyValuePair<string, object?>> sequence:
                    WriteObject(writer, StructuredObject.FromSequence(sequence));
                    break;

                case IEnumerable<object> enumerable:
                    writer.WriteStartArray();
                    foreach(var item in enumerable)
                    {
                        WriteValue(writer, item);
                    }
                    writer.WriteEndArray();
                    break;

                case System.Collections.IEnumerable enumerable:
                    writer.WriteStartArray();
                    foreach(var item in enumerable)
                    {
                        WriteValue(writer, item);
                    }
                    writer.WriteEndArray();
                    break;

                default:
                    WriteValue(writer, value.ToString());
                    break;

            }
        }        

        /// <summary>
        /// The file to write the json to
        /// </summary>
        public string? Filename{get; set;}

        /// <summary>
        /// True to overwrite the file.
        /// If false then if the file already exists an exception is thrown
        /// </summary>
        public bool Overwrite{get; set;}

        /// <summary>
        /// True to skip writing to file if the file already exists
        /// </summary>
        public bool SkipExisting{get; set;}
    }
}
