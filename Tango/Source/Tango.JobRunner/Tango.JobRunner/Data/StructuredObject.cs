using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Tango.JobRunner.Data
{
    public sealed class StructuredObject : IEnumerable<(string Name, object? Value)>
    {
        private readonly List<(string Name, object? Value)> m_Properties = new();
        private readonly Dictionary<string, int> m_NameIndex = new(StringComparer.OrdinalIgnoreCase);

        public void Add(string name, object? value)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("invalid name", nameof(name));

            if(m_NameIndex.ContainsKey(name)) throw new ArgumentException("name already exists", nameof(name));

            m_Properties.Add((name, value));
            m_NameIndex.Add(name, m_Properties.Count - 1);
        }

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

        public override string ToString()
        {
            return string.Join(", ", m_Properties.Select(pair => $"{pair.Name} = {pair.Value}"));
        }

        public IEnumerator<(string Name, object? Value)> GetEnumerator()
        {
            return m_Properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static StructuredObject From(object @object)
        {
            if(@object is null) throw new ArgumentNullException(nameof(@object));

            return @object switch
            {
                IEnumerable<KeyValuePair<string, object?>> pairs => FromSequence(pairs),
                var other => FromObject(other)
            };
        }

        /// <summary>
        /// Creates a structures object from any object
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static StructuredObject FromObject(object @object)
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
    }
}
