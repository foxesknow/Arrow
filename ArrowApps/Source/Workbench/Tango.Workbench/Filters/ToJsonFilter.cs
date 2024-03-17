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
    public sealed class ToJsonFilter : FileFilterBase
    {
        protected override async IAsyncEnumerable<object> WriteToFile(string filename, IAsyncEnumerable<object> items)
        {
            var options = new JsonWriterOptions()
            {
                Indented = this.Indented,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            using(var stream = File.Create(filename, 16384))
            {
                RegisterRollbackFile(filename);

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
            }
        }

        private void WriteObject(Utf8JsonWriter writer, object @object)
        {
            var structuredObject = ToStructuredObject(@object);

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
        /// True to indent the json
        /// </summary>
        public bool Indented{get; set;} = true;
    }
}
