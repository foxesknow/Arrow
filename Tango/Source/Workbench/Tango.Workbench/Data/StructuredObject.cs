using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Dynamic;

namespace Tango.Workbench.Data
{
    /// <summary>
    /// A dictionary type class that can be used to bundle related data together.
    /// For example, an instance may represent a row in a database
    /// </summary>
    public sealed class StructuredObject : IReadOnlyStructuredObject, ISupportDynamic
    {
        private readonly List<(string Name, object? Value)> m_Properties = new();
        private readonly Dictionary<string, int> m_NameIndex = new(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Adds a new value to the structured object object
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void Add(string name, object? value)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid name", nameof(name));

            if(m_NameIndex.ContainsKey(name)) throw new ArgumentException("name already exists", nameof(name));

            m_Properties.Add((name, value));
            m_NameIndex.Add(name, m_Properties.Count - 1);
        }

        /// <inheritdoc/>
        public object? this[string name]
        {
            get
            {
                if(m_NameIndex.TryGetValue(name, out var index))
                {
                    return m_Properties[index].Value;   
                }

                throw new IndexOutOfRangeException($"could not find {name}");
            }
        }

        /// <inheritdoc/>
        public object? this[int index]
        {
            get
            {
                return m_Properties[index].Value;
            }
        }

        /// <inheritdoc/>
        public int Count
        {
            get{return m_Properties.Count;}
        }

        /// <inheritdoc/>
        public bool TryGetValue(string name, out object? value)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));

            if(m_NameIndex.TryGetValue(name, out var index))
            {
                value = m_Properties[index].Value;
                return true;
            }

            value = null;
            return false;
        }

        /// <inheritdoc/>
        public ExpandoObject MakeExpandoObject()
        {
            var expando = new ExpandoObject();

            IDictionary<string, object?> dictionary = expando;
            foreach(var (key, value) in m_Properties)
            {
                dictionary.Add(key, value);
            }

            return expando;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return string.Join(", ", m_Properties.Select(pair => $"{pair.Name} = {pair.Value}"));
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
        {
            foreach(var (key, value) in m_Properties)
            {
                yield return new(key, value);
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Examines the incoming object and works out how best to create
        /// an instance that models the object
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static StructuredObject From(object @object)
        {
            if(@object is null) throw new ArgumentNullException(nameof(@object));

            return @object switch
            {
                var x when IsBasicValue(x)                       => FromValue(x),
                IEnumerable<KeyValuePair<string, object?>> pairs => FromSequence(pairs),
                var other                                        => FromObject(other)
            };
        }        

        /// <summary>
        /// Creates a structures object from a sequence
        /// </summary>
        /// <param name="pairs"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static StructuredObject FromSequence(IEnumerable<KeyValuePair<string, object?>> pairs)
        {
            if(pairs is null) throw new ArgumentNullException(nameof(pairs));

            var structuredObject = new StructuredObject();

            foreach(var pair in pairs)
            {
                structuredObject.Add(pair.Key, pair.Value);
            }

            return structuredObject;
        }

        /// <summary>
        /// Creates a structures object from any object via reflection
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static StructuredObject FromObject(object @object)
        {
            if(@object is null) throw new ArgumentNullException(nameof(@object));

            var structuredObject = new StructuredObject();

            foreach(var property in @object.GetType().GetProperties())
            {
                var getter = property.GetGetMethod();
                if(getter is null) continue;

                if(getter.IsPublic && getter.IsStatic == false && getter.GetParameters().Length == 0)
                {
                    var value = getter.Invoke(@object, null);
                    structuredObject.Add(property.Name, value);
                }
            }

            return structuredObject;
        }

        private static StructuredObject FromValue(object @object)
        {
            var structuredObject = new StructuredObject();
            structuredObject.Add("value", @object);

            return structuredObject;
        }

        private static bool IsBasicValue(object value)
        {
            switch(Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Boolean:
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.UInt16:
                case TypeCode.Int32:
                case TypeCode.UInt32:
                case TypeCode.Int64:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                case TypeCode.Char:
                    return true;

                default:    return false;
            }
        }
    }
}
