using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data.Mock
{
    public sealed partial class MockDatabaseDetails
    {
        private sealed class ParameterCollection : DbParameterCollection
        {
            private readonly List<DbParameter> m_Parameters = new();

            public override int Count
            {
                get{return m_Parameters.Count;}
            }

            public override object SyncRoot{get;} = new();

            public override int Add(object value)
            {
                var parameter = AsParameter(value);
                m_Parameters.Add(parameter);

                return m_Parameters.Count - 1;
            }

            public override void AddRange(Array values)
            {
                if(values is null) throw new ArgumentNullException(nameof(values));

                foreach(var value in values)
                {
                    Add(value);
                }
            }

            public override void Clear()
            {
                m_Parameters.Clear();
            }

            public override bool Contains(object value)
            {
                var parameter = AsParameter(value);
                return m_Parameters.Contains(parameter);
            }

            public override bool Contains(string value)
            {
                if(value is null) throw new ArgumentNullException(nameof(value));

                return m_Parameters.Any(parameter => parameter.ParameterName == value);
            }

            public override void CopyTo(Array array, int index)
            {
                if(array is null) throw new ArgumentNullException(nameof(array));

                System.Collections.ICollection untyped = m_Parameters;
                untyped.CopyTo(array, index);
            }

            public override IEnumerator GetEnumerator()
            {
                return m_Parameters.GetEnumerator();
            }

            public override int IndexOf(object value)
            {
                if(value is null) throw new ArgumentNullException(nameof(value));

                var parameter = AsParameter(value);
                return m_Parameters.IndexOf(parameter);
            }

            public override int IndexOf(string parameterName)
            {
                if(parameterName is null) throw new ArgumentNullException(nameof(parameterName));

                return m_Parameters.FindIndex(parameter => parameter.ParameterName == parameterName);
            }

            public override void Insert(int index, object value)
            {
                if(value is null) throw new ArgumentNullException(nameof(value));
        
                var parameter = AsParameter(value);
                m_Parameters.Insert(index, parameter);
            }

            public override void Remove(object value)
            {
                if(value is null) throw new ArgumentNullException(nameof(value));
        
                var parameter = AsParameter(value);
                m_Parameters.Remove(parameter);
            }

            public override void RemoveAt(int index)
            {
                m_Parameters.RemoveAt(index);
            }

            public override void RemoveAt(string parameterName)
            {
                if(IndexOf(parameterName) is int index && index != -1)
                {
                    m_Parameters.RemoveAt(index);
                }
            }

            protected override DbParameter GetParameter(int index)
            {
                return m_Parameters[index];
            }

            protected override DbParameter GetParameter(string parameterName)
            {
                if(IndexOf(parameterName) is int index && index != -1)
                {
                    return m_Parameters[index];
                }

                throw new DataException($"parameter not found: {parameterName}");
            }

            protected override void SetParameter(int index, DbParameter value)
            {
                if(value is null) throw new ArgumentNullException(nameof(value));

                m_Parameters[index] = value;
            }

            protected override void SetParameter(string parameterName, DbParameter value)
            {
                if(IndexOf(parameterName) is int index && index != -1)
                {
                    m_Parameters[index] = value;
                }

                throw new DataException($"could not find parameter: {parameterName}");
            }

            private DbParameter AsParameter(object value)
            {
                if(value is null) throw new ArgumentNullException(nameof(value));

                if(value is DbParameter parameter) return parameter;

                throw new ArgumentException("not a valid database parameter", nameof(value));
            }
        }
    }
}
