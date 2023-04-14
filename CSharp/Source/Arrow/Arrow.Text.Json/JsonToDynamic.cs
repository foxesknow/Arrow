using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Dynamic;

namespace Arrow.Text.Json
{
    /// <summary>
    /// Converts Json to a dynamic object.
    /// This can be useful for testing.
    /// </summary>
    public static class JsonToDynamic
    {
        /// <summary>
        /// Inflates the Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">json is null</exception>
        public static dynamic? Inflate(string json)
        {
            if(json is null) throw new ArgumentNullException(nameof(json));

            var doc = JsonDocument.Parse(json);
            return Walk(doc.RootElement);
        }

        private static dynamic? Walk(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Undefined     => null,
                JsonValueKind.Null          => null,
                JsonValueKind.False         => false,
                JsonValueKind.True          => true,
                JsonValueKind.String        => element.GetString(),
                JsonValueKind.Number        => ExtractNumber(element),
                JsonValueKind.Array         => WalkArray(element),
                JsonValueKind.Object        => WalkObject(element),
                _                           => throw new InvalidOperationException($"unexpected value: {element.ValueKind}")
            };
        }

        private static dynamic WalkObject(JsonElement element)
        {
            var expando = new ExpandoObject();            
            IDictionary<string, object?> dictionary = expando;

            foreach(var subElement in element.EnumerateObject())
            {
                dictionary.Add(subElement.Name, Walk(subElement.Value));
            }

            return expando;
        }

        private static List<object?> WalkArray(JsonElement element)
        {
            return new(element.EnumerateArray().Select(e => Walk(e)));
        }

        private static object ExtractNumber(JsonElement element)
        {
            if(element.TryGetInt32(out var i32)) return i32;
            if(element.TryGetInt64(out var i64)) return i64;
            if(element.TryGetDouble(out var @double)) return @double;
            if(element.TryGetDecimal(out var @decimal)) return @decimal;

            throw new InvalidCastException("unable to extract number");
        }
    }
}