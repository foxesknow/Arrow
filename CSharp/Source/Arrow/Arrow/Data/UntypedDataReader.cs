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
    /// This reader does not support type information
    /// </summary>
    public sealed class UntypedDataReader : SimpleDataReader
    {
        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        public UntypedDataReader(IReadOnlyList<string> columns, IEnumerable<object?[]> rows) : base(columns, rows)
        {
        }

        /// <inheritdoc/>
        protected override Type GetTypeForColumn(int i)
        {
            // We're untyped, so...
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a reader for a sequence of rows
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public static IDataReader Make(IReadOnlyList<string> columns, IEnumerable<object?[]> rows)
        {
            if(columns is null) throw new ArgumentNullException(nameof(columns));
            if(rows is null) throw new ArgumentNullException(nameof(rows));

            return new UntypedDataReader(columns, rows);
        }

        /// <summary>
        /// Creates a reader for a single row
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public static IDataReader MakeSingleRow(IReadOnlyList<string> columns, object?[] row)
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
        public static IDataReader MakeSingleColumn(string column, object?[] rowValues)
        {
            if(column is null) throw new ArgumentNullException(nameof(column));
            if(rowValues is null) throw new ArgumentNullException(nameof(rowValues));

            var rows = rowValues.Select(value => new[]{value})
                                .ToArray();
            
            return Make(new[]{column}, rows);
        }
    }
}
