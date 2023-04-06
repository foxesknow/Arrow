using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Arrow.Data
{
    /// <summary>
    /// An IDataReader that reads from a sequence of object arrays.
    /// 
    /// This reader supports type information
    /// </summary>
    public sealed class TypedDataReader : SimpleDataReader
    {
        private readonly IReadOnlyList<Type> m_ColumnTypes;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <exception cref="ArgumentException"></exception>
        public TypedDataReader(IReadOnlyList<(string Name, Type Type)> columns, IEnumerable<object?[]> rows) : base(columns.Select(c => c.Name).ToArray(), rows)
        {
            m_ColumnTypes = columns.Select(c => c.Type).ToArray();
            if(m_ColumnTypes.Any(type => type is null)) throw new ArgumentException("null type", nameof(columns));
        }

        protected override Type GetTypeForColumn(int i)
        {
            if(i >= 0 && i < m_ColumnTypes.Count)
            {
                return m_ColumnTypes[i];
            }

            throw new IndexOutOfRangeException();
        }

        /// <summary>
        /// Creates a reader for a sequence of rows
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static IDataReader Make(IReadOnlyList<(string Name, Type Type)> columns, IEnumerable<object?[]> rows)
        {
            if(columns is null) throw new ArgumentNullException(nameof(columns));
            if(rows is null) throw new ArgumentNullException(nameof(rows));

            return new TypedDataReader(columns, rows);
        }

        /// <summary>
        /// Creates a reader for a single row
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static IDataReader MakeSingleRow(IReadOnlyList<(string Name, Type Type)> columns, object?[] row)
        {
            if(columns is null) throw new ArgumentNullException(nameof(columns));
            if(row is null) throw new ArgumentNullException(nameof(row));
            if(row.Length != columns.Count) throw new ArgumentException("column and row size does not match");

            var rows = new[]{row};
            return Make(columns, rows);
        }

        /// <summary>
        /// Creates a readr for a single column
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rowValues"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IDataReader MakeSingleColumn((string Name, Type Type) column, object?[] rowValues)
        {
            if(rowValues is null) throw new ArgumentNullException(nameof(rowValues));
            if(column.Name is null) throw new ArgumentException("no name specified", nameof(column));
            if(column.Type is null) throw new ArgumentException("no type specified", nameof(column));

            var rows = rowValues.Select(value => new[]{value})
                                .ToArray();
            
            return Make(new[]{column}, rows);
        }
    }
}
