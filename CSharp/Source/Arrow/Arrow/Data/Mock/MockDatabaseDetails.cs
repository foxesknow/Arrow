using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data.Mock
{
    /// <summary>
    /// An implementation of DatabaseDetails that can be used to mock a real database
    /// </summary>
    public sealed partial class MockDatabaseDetails : DatabaseDetails
    {
        public delegate bool WhenPredicate(DbCommand command);
        public delegate T ThenFunction<T>(DbCommand command);

        private static readonly WhenPredicate AlwaysTrue = _ => true;

        private readonly List<(WhenPredicate When, ThenFunction<int>)> m_ExecuteNonQuery = new();
        private readonly List<(WhenPredicate When, ThenFunction<DbDataReader>)> m_ExecuteReader = new();
        private readonly List<(WhenPredicate When, ThenFunction<object?>)> m_ExecuteScalar = new();

        /// <summary>
        /// The connection string to use
        /// </summary>
        public string? ConnectionString{get; set;}   
        
        /// <summary>
        /// Creates a connection
        /// </summary>
        /// <returns></returns>
        /// <exception cref="DataException">Thrown if there is no connection string</exception>
        public override IDbConnection CreateConnection()
        {
            if(this.ConnectionString is null) throw new DataException("no connection string specified");

            return new Connection(this, this.ConnectionString);
        }

        /// <summary>
        /// Add a handler that will be called when ExecuteNonQuery is called
        /// </summary>
        /// <param name="then"></param>
        /// <returns></returns>
        public MockDatabaseDetails OnExecuteNonQuery(ThenFunction<int> then)
        {
            return OnExecuteNonQuery(AlwaysTrue, then);
        }

        /// <summary>
        /// Add a handler that will be called when ExecuteNonQuery is called
        /// if the "when" condition is true.
        /// </summary>
        /// <param name="when"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public MockDatabaseDetails OnExecuteNonQuery(WhenPredicate when, ThenFunction<int> then)
        {
            if(when is null) throw new ArgumentNullException(nameof(when));
            if(then is null) throw new ArgumentNullException(nameof(then));

            m_ExecuteNonQuery.Add((when, then));

            return this;
        }

        /// <summary>
        /// Add a handler that will be called when ExecuteReader is called
        /// </summary>
        /// <param name="then"></param>
        /// <returns></returns>
        public MockDatabaseDetails OnExecuteReader(ThenFunction<DbDataReader> then)
        {
            return OnExecuteReader(AlwaysTrue, then);
        }

        /// <summary>
        /// Add a handler that will be called when ExecuteReader is called
        /// if the "when" condition is true,
        /// </summary>
        /// <param name="when"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public MockDatabaseDetails OnExecuteReader(WhenPredicate when, ThenFunction<DbDataReader> then)
        {
            if(when is null) throw new ArgumentNullException(nameof(when));
            if(then is null) throw new ArgumentNullException(nameof(then));

            m_ExecuteReader.Add((when, then));

            return this;
        }

        /// <summary>
        /// Add a handler that will be called when ExecuteScalar is called
        /// </summary>
        /// <param name="then"></param>
        /// <returns></returns>
        public MockDatabaseDetails OnExecuteScalar(ThenFunction<object?> then)
        {
            return OnExecuteScalar(AlwaysTrue, then);
        }

        /// <summary>
        /// Add a handler that will be called when ExecuteScalar is called
        /// if the "when" condition is true.
        /// </summary>
        /// <param name="when"></param>
        /// <param name="then"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public MockDatabaseDetails OnExecuteScalar(WhenPredicate when, ThenFunction<object?> then)
        {
            if(when is null) throw new ArgumentNullException(nameof(when));
            if(then is null) throw new ArgumentNullException(nameof(then));

            m_ExecuteScalar.Add((when, then));

            return this;
        }

        private int ExecuteNonQuery(DbCommand command)
        {
            foreach(var (when, then) in m_ExecuteNonQuery)
            {
                if(when(command)) return then(command);
            }

            throw new DataException($"no handler for {nameof(ExecuteNonQuery)}");
        }

        private DbDataReader ExecuteReader(DbCommand command)
        {
            foreach(var (when, then) in m_ExecuteReader)
            {
                if(when(command)) return then(command);
            }

            throw new DataException($"no handler for {nameof(ExecuteReader)}");
        }

        private object? ExecuteScalar(DbCommand command)
        {
            foreach(var (when, then) in m_ExecuteScalar)
            {
                if(when(command)) return then(command);
            }

            throw new DataException($"no handler for {nameof(ExecuteScalar)}");
        }
    }
}
