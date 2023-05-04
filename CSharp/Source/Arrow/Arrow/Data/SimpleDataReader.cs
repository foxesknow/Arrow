using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.Common;

namespace Arrow.Data
{
    /// <summary>
    /// Base class for in memory data readers
    /// </summary>
    public abstract class SimpleDataReader : DataReaderBase
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

        protected abstract Type GetTypeForColumn(int i);

        /// <inheritdoc/>
        public override int Depth{get;} = 0;

        /// <inheritdoc/>
        public override int FieldCount
        {
            get{return m_Columns.Count;}
        }

        /// <inheritdoc/>
        public override bool HasRows => throw new NotImplementedException();

        /// <inheritdoc/>
        public override bool IsClosed
        {
            get{return m_IsClosed;}
        }

        /// <inheritdoc/>
        public override int RecordsAffected 
        {
            get{return -1;}
        }

        /// <inheritdoc/>
        public override object this[string name]
        {
            get
            {
                if(m_ColumnsToIndex.TryGetValue(name, out var ordinal))
                {
                    return this[ordinal];
                }

                throw new IndexOutOfRangeException($"column not found: {name}");
            }
        }

        /// <inheritdoc/>
        public override object this[int ordinal]
        {
            get{return GetCurrentValue(ordinal);}
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

        /// <inheritdoc/>
        public override bool GetBoolean(int ordinal)
        {
            return Convert.ToBoolean(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override byte GetByte(int ordinal)
        {
            return Convert.ToByte(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length)
        {
            var data = (byte[])GetCurrentValue(ordinal);
            return GetArray(data, dataOffset, buffer, bufferOffset, length);
        }

        /// <inheritdoc/>
        public override char GetChar(int ordinal)
        {
            return Convert.ToChar(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length)
        {
            var data = (char[])GetCurrentValue(ordinal);
            return GetArray(data, dataOffset, buffer, bufferOffset, length);
        }

        /// <inheritdoc/>
        public override string GetDataTypeName(int ordinal)
        {
            return GetTypeForColumn(ordinal).Name;
        }

        /// <inheritdoc/>
        public override DateTime GetDateTime(int ordinal)
        {
            return Convert.ToDateTime(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override decimal GetDecimal(int ordinal)
        {
            return Convert.ToDecimal(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override double GetDouble(int ordinal)
        {
            return Convert.ToDouble(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override IEnumerator GetEnumerator()
        {
            return new DbEnumerator(this);
        }

        /// <inheritdoc/>
        public override Type GetFieldType(int ordinal)
        {
            return GetTypeForColumn(ordinal);
        }

        /// <inheritdoc/>
        public override float GetFloat(int ordinal)
        {
            return Convert.ToSingle(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override Guid GetGuid(int ordinal)
        {
            return Guid.Parse(GetCurrentValue(ordinal).ToString()!);
        }

        /// <inheritdoc/>
        public override short GetInt16(int ordinal)
        {
            return Convert.ToInt16(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override int GetInt32(int ordinal)
        {
            return Convert.ToInt32(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override long GetInt64(int ordinal)
        {
            return Convert.ToInt64(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override string GetName(int ordinal)
        {
            if(ordinal >= 0 && ordinal < m_Columns.Count)
            {
                return m_Columns[ordinal];
            }

            throw new IndexOutOfRangeException();
        }

        /// <inheritdoc/>
        public override int GetOrdinal(string name)
        {
            if(m_ColumnsToIndex.TryGetValue(name, out var index)) return index;

            throw new IndexOutOfRangeException($"no such column: {name}");
        }

        /// <inheritdoc/>
        public override string GetString(int ordinal)
        {
            return (string)GetCurrentValue(ordinal);
        }

        /// <inheritdoc/>
        public override object GetValue(int ordinal)
        {
            return GetCurrentValue(ordinal);
        }

        /// <inheritdoc/>
        public override int GetValues(object[] values)
        {
            var row = m_Enumerator.Current;
            int count = Math.Min(values.Length, row.Length);
            Array.Copy(row, values, count);

            return count;
        }

        /// <inheritdoc/>
        public override bool IsDBNull(int ordinal)
        {
            return Convert.IsDBNull(GetCurrentValue(ordinal));
        }

        /// <inheritdoc/>
        public override bool NextResult()
        {
            return false;
        }

        /// <inheritdoc/>
        public override bool Read()
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

        /// <inheritdoc/>
        public override void Close()
        {
            base.Close();
            m_IsClosed = true;
        }

        /// <inheritdoc/>
        public override DataTable? GetSchemaTable()
        {
            return null;
        }
    }
}
