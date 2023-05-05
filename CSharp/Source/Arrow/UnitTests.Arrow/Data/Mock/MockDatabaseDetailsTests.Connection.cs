using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;

using NUnit.Framework;

namespace UnitTests.Arrow.Data.Mock
{
    public partial class MockDatabaseDetailsTests
    {
        [Test]
        public void Connection_Properties()
        {
            var details = MakeDetails();
            using(var connection = details.CreateConnection())
            {
                Assert.That(connection.ConnectionString, Is.Not.Null & Has.Length.GreaterThan(0));
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Closed));

                connection.Open();
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            }
        }

        [Test]
        public void BeginTransaction()
        {
            var details = MakeDetails();
            using(var connection = details.CreateConnection())
            {
                connection.Open();
                using(var transaction = connection.BeginTransaction())
                {
                    Assert.That(transaction, Is.Not.Null);
                    Assert.That(transaction.Connection, Is.SameAs(connection));
                }
            }
        }

        [Test]
        public void BeginTransaction_NotOpen()
        {
            var details = MakeDetails();
            using(var connection = details.CreateConnection())
            {
                Assert.Catch(() => connection.BeginTransaction());
            }
        }
    }
}
