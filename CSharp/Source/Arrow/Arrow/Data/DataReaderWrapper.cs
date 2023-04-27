using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Arrow.Execution;
using System.IO;

namespace Arrow.Data
{
    /// <summary>
    /// A data reader wrapper that makes all IDataReader calls virtual so that
    /// you can override whatever is appropriate.
    /// </summary>
    public class DataReaderWrapper : IDataReader, IWrapper<IDataReader>
    {
        private readonly IDataReader m_Reader;
        private readonly IDbCommand? m_Command;

        private readonly DataReaderWrapperCloseMode m_CloseMode;

        /// <summary>
        /// Initializes the instance with a reader that will be closed
        /// </summary>
        /// <param name="reader"></param>
        public DataReaderWrapper(IDataReader reader) : this(DataReaderWrapperCloseMode.Reader, reader)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="closeMode"></param>
        /// <param name="reader"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DataReaderWrapper(DataReaderWrapperCloseMode closeMode, IDataReader reader)
        {
            if(reader is null) throw new ArgumentNullException(nameof(reader));

            m_CloseMode = closeMode;
            m_Reader = reader;
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="closeMode"></param>
        /// <param name="command"></param>
        /// <param name="reader"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DataReaderWrapper(DataReaderWrapperCloseMode closeMode, IDbCommand command, IDataReader reader)
        {
            if(command is null) throw new ArgumentNullException(nameof(command));
            if(reader is null) throw new ArgumentNullException(nameof(reader));

            m_CloseMode = closeMode;
            m_Command = command;
            m_Reader = reader;
        }

        /// <inheritdoc/>
        IDataReader IWrapper<IDataReader>.WrappedItem
        {
            get{return m_Reader;}
        }

        /// <inheritdoc/>
        public virtual object this[int i]
        {
            get{return m_Reader[i];}
        }

        /// <inheritdoc/>
        public virtual object this[string name]
        {
            get{return m_Reader[name];}
        }

        /// <inheritdoc/>
        public virtual int Depth
        {
            get{return m_Reader.Depth;}
        }

        /// <inheritdoc/>
        public virtual bool IsClosed
        {
            get{return m_Reader.IsClosed;}
        }

        /// <inheritdoc/>
        public virtual int RecordsAffected
        {
            get{return m_Reader.RecordsAffected;}
        }

        /// <inheritdoc/>
        public virtual int FieldCount
        {
            get{return m_Reader.FieldCount;}
        }

        /// <inheritdoc/>
        public virtual void Close()
        {
            if(m_CloseMode >= DataReaderWrapperCloseMode.Reader)
            {
                m_Reader.Close();
            }

            var connection = m_Command?.Connection;

            if(m_CloseMode >= DataReaderWrapperCloseMode.Command && m_Command is not null)
            {
                DisposeOfParameters(m_Command.Parameters);
                m_Command.Dispose();
            }

            if(m_CloseMode >= DataReaderWrapperCloseMode.Connection && connection is not null)
            {
                connection.Close();
            }
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            if(m_CloseMode >= DataReaderWrapperCloseMode.Reader)
            {
                m_Reader.Dispose();
            }

            var connection = m_Command?.Connection;

            if(m_CloseMode >= DataReaderWrapperCloseMode.Command && m_Command is not null)
            {
                DisposeOfParameters(m_Command.Parameters);
                m_Command.Dispose();
            }

            if(m_CloseMode >= DataReaderWrapperCloseMode.Connection && connection is not null)
            {
                connection.Dispose();
            }
        }

        /// <inheritdoc/>
        public virtual bool GetBoolean(int i)
        {
            return m_Reader.GetBoolean(i);
        }

        /// <inheritdoc/>
        public virtual byte GetByte(int i)
        {
            return m_Reader.GetByte(i);
        }

        /// <inheritdoc/>
        public virtual long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return m_Reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /// <inheritdoc/>
        public virtual char GetChar(int i)
        {
            return m_Reader.GetChar(i);
        }

        /// <inheritdoc/>
        public virtual long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return m_Reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        /// <inheritdoc/>
        public virtual IDataReader GetData(int i)
        {
            return m_Reader.GetData(i);
        }

        /// <inheritdoc/>
        public virtual string GetDataTypeName(int i)
        {
            return m_Reader.GetDataTypeName(i);
        }

        /// <inheritdoc/>
        public virtual DateTime GetDateTime(int i)
        {
            return m_Reader.GetDateTime(i);
        }

        /// <inheritdoc/>
        public virtual decimal GetDecimal(int i)
        {
            return m_Reader.GetDecimal(i);
        }

        /// <inheritdoc/>
        public virtual double GetDouble(int i)
        {
            return m_Reader.GetDouble(i);
        }

        /// <inheritdoc/>
        public virtual Type GetFieldType(int i)
        {
            return m_Reader.GetFieldType(i);
        }

        /// <inheritdoc/>
        public virtual float GetFloat(int i)
        {
            return m_Reader.GetFloat(i);
        }

        /// <inheritdoc/>
        public virtual Guid GetGuid(int i)
        {
            return m_Reader.GetGuid(i);
        }

        /// <inheritdoc/>
        public virtual short GetInt16(int i)
        {
            return m_Reader.GetInt16(i);
        }

        /// <inheritdoc/>
        public virtual int GetInt32(int i)
        {
            return m_Reader.GetInt32(i);
        }

        /// <inheritdoc/>
        public virtual long GetInt64(int i)
        {
            return m_Reader.GetInt64(i);
        }

        /// <inheritdoc/>
        public virtual string GetName(int i)
        {
            return m_Reader.GetName(i);
        }

        /// <inheritdoc/>
        public virtual int GetOrdinal(string name)
        {
            return m_Reader.GetOrdinal(name);
        }

        /// <inheritdoc/>
        public virtual DataTable GetSchemaTable()
        {
            return m_Reader.GetSchemaTable();
        }

        /// <inheritdoc/>
        public virtual string GetString(int i)
        {
            return m_Reader.GetString(i);
        }

        /// <inheritdoc/>
        public virtual object GetValue(int i)
        {
            return m_Reader.GetValue(i);
        }

        /// <inheritdoc/>
        public virtual int GetValues(object[] values)
        {
            return m_Reader.GetValues(values);
        }

        /// <inheritdoc/>
        public virtual bool IsDBNull(int i)
        {
            return m_Reader.IsDBNull(i);
        }

        /// <inheritdoc/>
        public virtual bool NextResult()
        {
            return m_Reader.NextResult();
        }

        /// <inheritdoc/>
        public virtual bool Read()
        {
            return m_Reader.Read();
        }

        /// <summary>
        /// Some database providers, like Oracle, have parameters that implement IDisposable
        /// </summary>
        /// <param name="parameters"></param>
        private void DisposeOfParameters(IDataParameterCollection? parameters)
        {
            if(parameters is null) return;

            foreach(var parameter in parameters)
            {
                if(parameter is IDisposable d) d.Dispose();
            }
        }
    }
}
