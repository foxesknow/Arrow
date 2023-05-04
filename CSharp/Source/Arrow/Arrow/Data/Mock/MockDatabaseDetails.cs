using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Data.Mock
{
    public sealed partial class MockDatabaseDetails : DatabaseDetails
    {
        private static readonly WhenPredicate AlwaysTrue = _ => true;

        private readonly List<(WhenPredicate When, ThenFunction<int>)> m_ExecuteNonQuery = new();
        private readonly List<(WhenPredicate When, ThenFunction<IDataReader>)> m_ExecuteReader = new();
        private readonly List<(WhenPredicate When, ThenFunction<object?>)> m_ExecuteScalar = new();

        public string? ConnectionString{get; set;}   
        
        public override IDbConnection CreateConnection()
        {
            if(this.ConnectionString is null) throw new DataException("no connection string specified");

            return new Connection(this, this.ConnectionString);
        }

        public MockDatabaseDetails OnExecuteNonQuery(ThenFunction<int> then)
        {
            return OnExecuteNonQuery(AlwaysTrue, then);
        }

        public MockDatabaseDetails OnExecuteNonQuery(WhenPredicate when, ThenFunction<int> then)
        {
            if(when is null) throw new ArgumentNullException(nameof(when));
            if(then is null) throw new ArgumentNullException(nameof(then));

            m_ExecuteNonQuery.Add((when, then));

            return this;
        }

        public MockDatabaseDetails OnExecuteReader(ThenFunction<IDataReader> then)
        {
            return OnExecuteReader(AlwaysTrue, then);
        }

        public MockDatabaseDetails OnExecuteReader(WhenPredicate when, ThenFunction<IDataReader> then)
        {
            if(when is null) throw new ArgumentNullException(nameof(when));
            if(then is null) throw new ArgumentNullException(nameof(then));

            m_ExecuteReader.Add((when, then));

            return this;
        }

        public MockDatabaseDetails OnExecuteScalar(ThenFunction<object?> then)
        {
            return OnExecuteScalar(AlwaysTrue, then);
        }

        public MockDatabaseDetails OnExecuteScalar(WhenPredicate when, ThenFunction<object?> then)
        {
            if(when is null) throw new ArgumentNullException(nameof(when));
            if(then is null) throw new ArgumentNullException(nameof(then));

            m_ExecuteScalar.Add((when, then));

            return this;
        }
    }
}
