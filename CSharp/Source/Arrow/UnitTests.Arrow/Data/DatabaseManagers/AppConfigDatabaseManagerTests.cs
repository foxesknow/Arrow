using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Data;
using Arrow.Data.DatabaseManagers;

using NUnit.Framework;

namespace UnitTests.Arrow.Data.DatabaseManagers
{
    [TestFixture]
    public class AppConfigDatabaseManagerTests
    {
        [Test]
        public void Initialize_NullFactory()
        {
            Assert.Catch(() => new AppConfigDatabaseManager(null));
        }

        [Test]
        public void Initialize()
        {
            var manager = Make();
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(2));
            Assert.That(manager.IsTransactional("Prod"), Is.False);
            Assert.That(manager.IsDynamic("Prod"), Is.False);
            Assert.That(manager.IsTransactional("Uat"), Is.True);
            Assert.That(manager.IsDynamic("Uat"), Is.False);
        }

        [Test]
        public void Initialize_OnlyProd()
        {
            IDatabaseManager manager = new AppConfigDatabaseManager(DatabaseFactory, config => config.Name == "Prod");
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(1));
            Assert.That(manager.IsTransactional("Prod"), Is.False);
            Assert.That(manager.IsDynamic("Prod"), Is.False);
        }

        [Test]
        public void OpenDynamicConnection()
        {
            var manager = Make();
            var arguments = new Dictionary<string, object>()
            {
                {"username", "Jack"}
            };

            Assert.Catch(() => manager.OpenDynamicConnection("Prod", arguments));
        }

        [Test]
        public void OpenConnection_NotFound()
        {
            var manager = Make();
            Assert.Catch(() => manager.OpenConnection("Foo"));
        }

        [Test]
        public void GetConnectionInfo_NotFound()
        {
            var manager = Make();
            Assert.Catch(() => manager.GetConnectionInfo("Foo"));
        }

        private IDatabaseManager Make()
        {
            return new AppConfigDatabaseManager(DatabaseFactory);
        }

        private (Database Database, ConnectionInfo ConnectionInfo) DatabaseFactory(ConnectionStringSettings connectionStringSettings)
        {
            if(connectionStringSettings.Name == "Prod")
            {
                Assert.That(connectionStringSettings.ConnectionString, Is.EqualTo("Island"));
                Assert.That(connectionStringSettings.ProviderName, Is.EqualTo("Null"));

                var database = new Database()
                {
                    Name = connectionStringSettings.Name,
                    Transactional = false,
                    Details = new NullDatabaseDetails()
                    {
                        ConnectionString = connectionStringSettings.ConnectionString
                    }
                };

                return (database, ConnectionInfo.Default);
            }

            if(connectionStringSettings.Name == "Uat")
            {
                Assert.That(connectionStringSettings.ConnectionString, Is.EqualTo("Orchid"));
                Assert.That(connectionStringSettings.ProviderName, Is.EqualTo("Oracle"));

                var database = new Database()
                {
                    Name = connectionStringSettings.Name,
                    Transactional = true,
                    Details = new NullDatabaseDetails()
                    {
                        ConnectionString = connectionStringSettings.ConnectionString
                    }
                };

                return (database, ConnectionInfo.Transactional);
            }
            

            Assert.Fail();
            return default;
        }
    }
}
