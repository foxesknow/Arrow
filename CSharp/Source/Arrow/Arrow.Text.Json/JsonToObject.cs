using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Arrow.Text.Json
{
    /// <summary>
    /// Converts Json to an object.
    /// </summary>
    public sealed class JsonToObject
    {
        private static readonly Func<JsonElement, object> s_ConvertToNumber = DefaultConvertToNumber;
        private static readonly Func<IDictionary<string, object?>> s_ObjectFactory = DefaultObjectFactory;
        private static readonly Func<IList<object?>> s_ArrayFactory = DefaultArrayFactory;

        private Func<JsonElement, object> m_ConvertToNumber;
        private Func<IDictionary<string, object?>> m_ObjectFactory;
        private Func<IList<object?>> m_ArrayFactory;
   
        public JsonToObject()
        {
            m_ConvertToNumber = s_ConvertToNumber;
            m_ObjectFactory = s_ObjectFactory;
            m_ArrayFactory = s_ArrayFactory;
        }

        /// <summary>
        /// Converts a Json number to a .NET number
        /// </summary>
        public Func<JsonElement, object> ConvertToNumber
        {
            get{return m_ConvertToNumber;}
            set
            {
                if(value is null) throw new ArgumentNullException(nameof(value));
                m_ConvertToNumber = value;
            }
        }

        /// <summary>
        /// Creates the dictionaries that will be used to model objects
        /// </summary>
        public Func<IDictionary<string, object?>> ObjectFactory
        {
            get{return m_ObjectFactory;}
            set
            {
                if(value is null) throw new ArgumentNullException(nameof(value));
                m_ObjectFactory = value;
            }
        }

        /// <summary>
        /// Inflates the Json
        /// </summary>
        /// <param name="json"></param>
        /// <returns>An object representing the Json</returns>
        /// <exception cref="ArgumentNullException">json is null</exception>
        public object? Inflate(string json)
        {
            if(json is null) throw new ArgumentNullException(nameof(json));

            var doc = JsonDocument.Parse(json);
            return Walk(doc.RootElement);
        }

        /// <summary>
        /// Creates the lists that will be model arrays
        /// </summary>
        public Func<IList<object?>> ArrayFactory
        {
            get{return m_ArrayFactory;}
            set
            {
                if(value is null) throw new ArgumentNullException(nameof(value));
                m_ArrayFactory = value;
            }
        }

        private object? Walk(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Undefined     => null,
                JsonValueKind.Null          => null,
                JsonValueKind.False         => false,
                JsonValueKind.True          => true,
                JsonValueKind.String        => element.GetString(),
                JsonValueKind.Number        => this.ConvertToNumber(element),
                JsonValueKind.Array         => WalkArray(element),
                JsonValueKind.Object        => WalkObject(element),
                _                           => throw new InvalidOperationException($"unexpected value: {element.ValueKind}")
            };
        }

        private IDictionary<string, object?> WalkObject(JsonElement element)
        {
            IDictionary<string, object?> dictionary = this.ObjectFactory();

            foreach(var subElement in element.EnumerateObject())
            {
                dictionary.Add(subElement.Name, Walk(subElement.Value));
            }

            return dictionary;
        }

        private IList<object?> WalkArray(JsonElement element)
        {
            var list = this.ArrayFactory();

            foreach(var arrayElement in element.EnumerateArray())
            {
                var value = Walk(arrayElement);
                list.Add(value);
            }

            return list;
        }

        private static object DefaultConvertToNumber(JsonElement element)
        {
            if(element.TryGetInt32(out var i32)) return i32;
            if(element.TryGetInt64(out var i64)) return i64;
            if(element.TryGetDouble(out var @double)) return @double;
            if(element.TryGetDecimal(out var @decimal)) return @decimal;

            throw new InvalidCastException("unable to extract number");
        }

        private static IDictionary<string, object?> DefaultObjectFactory()
        {
            return new Dictionary<string, object?>();
        }

        private static IList<object?> DefaultArrayFactory()
        {
            return new List<object?>();
        }
    }
}
