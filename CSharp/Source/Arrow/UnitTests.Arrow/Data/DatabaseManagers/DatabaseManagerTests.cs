using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Data;
using Arrow.Data.DatabaseManagers;

using NUnit.Framework;

namespace UnitTests.Arrow.Data.DatabaseManagers
{
    [TestFixture]
    public class DatabaseManagerTests
    {
        [Test]
        public void Initialization()
        {
            IDatabaseManager manager = new DatabaseManager();
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(0));
        }

        [Test]
        public void Add()
        {
            var m = new DatabaseManager();            
            m.Add("Island", TransactionMode.NonTransactional, new NullDatabaseDetails());

            IDatabaseManager manager = m;
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(1));
            Assert.That(manager.NonDynamicDatabaseName().Count(), Is.EqualTo(1));

            Assert.That(manager.HasDatabase("Island"), Is.True);
            Assert.That(manager.HasDatabase("island"), Is.True);
            Assert.That(manager.HasDatabase("ISLAND"), Is.True);
        }

        [Test]
        public void Add_Duplicate()
        {
            var m = new DatabaseManager();            
            m.Add("Island", TransactionMode.NonTransactional, new NullDatabaseDetails());
            Assert.Catch(() => m.Add("Island", TransactionMode.NonTransactional, new NullDatabaseDetails()));
        }

        [Test]
        public void Add_Dynamic()
        {
            var m = new DatabaseManager();            
            m.AddDynamic("Island", TransactionMode.NonTransactional, args => new NullDatabaseDetails());

            IDatabaseManager manager = m;
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(1));
            Assert.That(manager.NonDynamicDatabaseName().Count(), Is.EqualTo(0));

            Assert.That(manager.HasDatabase("Island"), Is.True);
            Assert.That(manager.HasDatabase("island"), Is.True);
            Assert.That(manager.HasDatabase("ISLAND"), Is.True);
        }

        [Test]
        public void Add_Dynamic_Duplicate()
        {
            var m = new DatabaseManager();            
            m.AddDynamic("Island", TransactionMode.NonTransactional, args => new NullDatabaseDetails());            Assert.Catch(() => m.AddDynamic("Island", TransactionMode.NonTransactional, args => new NullDatabaseDetails()));

        }

        [Test]
        public void OpenConnection()
        {
            var m = new DatabaseManager();            
            m.Add("Island", TransactionMode.NonTransactional, new NullDatabaseDetails());

            IDatabaseManager manager = m;
            using(var connection = manager.OpenConnection("Island"))
            {
                Assert.That(connection, Is.Not.Null);
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            }
        }

        [Test]
        public void OpenConnection_NotFound()
        {
            var m = new DatabaseManager();            
            m.Add("Island", TransactionMode.NonTransactional, new NullDatabaseDetails());

            IDatabaseManager manager = m;
            Assert.Catch(() => manager.OpenConnection("foo"));
        }

        [Test]
        public void OpenDynamicConnection()
        {
            var factoryCallCount = 0;
            var m = new DatabaseManager();            
            m.AddDynamic("Island", TransactionMode.NonTransactional, args => 
            {
                factoryCallCount++;
                return new NullDatabaseDetails();
            });

            var arguments = new Dictionary<string, object>();

            IDatabaseManager manager = m;
            using(var connection = manager.OpenDynamicConnection("island", arguments))
            {
                Assert.That(connection, Is.Not.Null);
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
                Assert.That(factoryCallCount, Is.EqualTo(1));
            }

            using(var connection = manager.OpenDynamicConnection("island", arguments))
            {
                Assert.That(connection, Is.Not.Null);
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
                Assert.That(factoryCallCount, Is.EqualTo(2));
            }
        }

        [Test]
        public void GetConnectionInfo()
        {
            var m = new DatabaseManager();            
            m.Add("Island", TransactionMode.NonTransactional, new NullDatabaseDetails());
            m.AddDynamic("London", TransactionMode.Transactional, args => new NullDatabaseDetails());

            IDatabaseManager manager = m;
            Assert.That(manager.GetConnectionInfo("island"), Is.EqualTo(ConnectionInfo.Default));
            Assert.That(manager.GetConnectionInfo("london"), Is.EqualTo(ConnectionInfo.Transactional | ConnectionInfo.Dynamic));
        }
    }
}
