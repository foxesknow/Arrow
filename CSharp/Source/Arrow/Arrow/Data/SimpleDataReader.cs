using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Arrow.Data
{
    public abstract class SimpleDataReader : DataReaderBase, IDataReader
    {
        private readonly IReadOnlyDictionary<string, int> m_ColumnsToIndex;
        private readonly IReadOnlyList<string> m_Columns;

        private readonly IEnumerator<object?[]> m_Enumerator;
        private bool m_IsClosed;

        protected SimpleDataReader(IReadOnlyList<string> columns, IEnumerable<object?[]> rows)
        {
            if(columns is null) throw new ArgumentNullException(nameof(columns));
            if(rows is null) throw new ArgumentNullException(nameof(rows));

            var columnsToIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for(var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                if(string.IsNullOrWhiteSpace(column)) throw new ArgumentException("invalid column name in sequence", nameof(columns));

                // It's valid to have columns with the same name (surprise)
                if(columnsToIndex.ContainsKey(column) == false)
                {
                    columnsToIndex.Add(column, i);
                }
            }

            m_ColumnsToIndex = columnsToIndex;
            m_Columns = columns.ToList();
            m_Enumerator = rows.GetEnumerator();
        }

        object IDataRecord.this[int i]
        {
            get{return GetCurrentValue(i);}
        }

        object IDataRecord.this[string name]
        {
            get
            {
                if(m_ColumnsToIndex.TryGetValue(name, out var index))
                {
                    return GetCurrentValue(index);
                }
                else
                {
                    throw new IndexOutOfRangeException($"no such column: {name}");
                }
            }
        }

        int IDataReader.Depth
        {
            get{return 0;}
        }

        bool IDataReader.IsClosed
        {
            get{return m_IsClosed;}
        }

        int IDataReader.RecordsAffected
        {
            get{return -1;}
        }

        int IDataRecord.FieldCount
        {
            get{return m_Columns.Count;}
        }

        void IDataReader.Close()
        {
            ((IDisposable)this).Dispose();
        }

        void IDisposable.Dispose()
        {
            m_Enumerator.Dispose();
            m_IsClosed = true;
        }

        bool IDataRecord.GetBoolean(int i)
        {
            return Convert.ToBoolean(GetCurrentValue(i));
        }

        byte IDataRecord.GetByte(int i)
        {
            return Convert.ToByte(GetCurrentValue(i));
        }

        long IDataRecord.GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length)
        {
            var data = (byte[])GetCurrentValue(i);
            return GetArray(data, fieldOffset, buffer, bufferoffset, length);
        }

        char IDataRecord.GetChar(int i)
        {
            return Convert.ToChar(GetCurrentValue(i));
        }

        long IDataRecord.GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length)
        {
            var data = (char[])GetCurrentValue(i);
            return GetArray(data, fieldoffset, buffer, bufferoffset, length);
        }

        IDataReader IDataRecord.GetData(int i)
        {
            throw new NotImplementedException();
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            var type = ((IDataRecord)this).GetFieldType(i);
            return type.Name;
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            return Convert.ToDateTime(GetCurrentValue(i));
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            return Convert.ToDecimal(GetCurrentValue(i));
        }

        double IDataRecord.GetDouble(int i)
        {
            return Convert.ToDouble(GetCurrentValue(i));
        }

        Type IDataRecord.GetFieldType(int i)
        {
            return GetTypeForColumn(i);
        }

        float IDataRecord.GetFloat(int i)
        {
            return Convert.ToSingle(GetCurrentValue(i));
        }

        Guid IDataRecord.GetGuid(int i)
        {
            return Guid.Parse(GetCurrentValue(i).ToString()!);
        }

        short IDataRecord.GetInt16(int i)
        {
            return Convert.ToInt16(GetCurrentValue(i));
        }

        int IDataRecord.GetInt32(int i)
        {
            return Convert.ToInt32(GetCurrentValue(i));
        }

        long IDataRecord.GetInt64(int i)
        {
            return Convert.ToInt64(GetCurrentValue(i));
        }

        string IDataRecord.GetName(int i)
        {
            if(i >= 0 && i < m_Columns.Count)
            {
                return m_Columns[i];
            }

            throw new IndexOutOfRangeException();
        }

        int IDataRecord.GetOrdinal(string name)
        {
            if(m_ColumnsToIndex.TryGetValue(name, out var index)) return index;

            throw new IndexOutOfRangeException($"no such column: {name}");
        }

        DataTable? IDataReader.GetSchemaTable()
        {
            return MakeSchemaTable(this);
        }

        string IDataRecord.GetString(int i)
        {
            return (string)GetCurrentValue(i);
        }

        object IDataRecord.GetValue(int i)
        {
            return GetCurrentValue(i);
        }

        int IDataRecord.GetValues(object[] values)
        {
            var row = m_Enumerator.Current;
            int count = Math.Min(values.Length, row.Length);
            Array.Copy(row, values, count);

            return count;
        }

        bool IDataRecord.IsDBNull(int i)
        {
            return Convert.IsDBNull(GetCurrentValue(i));
        }

        bool IDataReader.NextResult()
        {
            return false;
        }

        bool IDataReader.Read()
        {
            var read = m_Enumerator.MoveNext();

            if(read)
            {
                // Make sure the row is valid, to avoid any nasty surprises later
                var row = m_Enumerator.Current;
                if(row is null) throw new InvalidOperationException("null row in sequence");
                if(row.Length != m_Columns.Count) throw new InvalidOperationException($"expected {m_Columns.Count} columns, got {row.Length}");
            }

            return read;
        }

        private object GetCurrentValue(int index)
        {
            var row = m_Enumerator.Current;

            if(index >= 0 && index < row.Length)
            {
                var value = row[index];
                return value is null ? DBNull.Value : value;
            }
            else
            {
                throw new IndexOutOfRangeException();
            }
        }

        protected abstract Type GetTypeForColumn(int i);
    }
}
