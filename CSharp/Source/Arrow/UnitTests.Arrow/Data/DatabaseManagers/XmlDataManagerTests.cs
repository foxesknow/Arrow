using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Arrow.Xml.ObjectCreation;
using Arrow.Data;
using Arrow.Data.DatabaseManagers;

using NUnit.Framework;
using System.ComponentModel;
using System.Data;

namespace UnitTests.Arrow.Data.DatabaseManagers
{
    [TestFixture]
    public class XmlDataManagerTests
    {
        [Test]
        public void EmptyManager()
        {
            var manager = Make(_ => {});
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(0));
            Assert.That(manager.NonDynamicDatabaseName().Count(), Is.EqualTo(0));
        }

        [Test]
        public void DuplicateNames()
        {
            Assert.Catch(() => Make(s_DuplicateRegularDatabases));
        }

        [Test]
        public void DuplicateDynamicNames()
        {
            Assert.Catch(() => Make(s_DuplicateDynamicDatabases));
        }

        [Test]
        public void GetConnectionInfo()
        {
            var manager = Make(s_SingleRegularDatabases);
            Assert.That(manager.GetConnectionInfo("Island"), Is.EqualTo(ConnectionInfo.Default));
            Assert.That(manager.GetConnectionInfo("ISLAND"), Is.EqualTo(ConnectionInfo.Default));
            
            Assert.Catch(() => manager.GetConnectionInfo("foo"));
        }

        [Test]
        public void SingleDatabase()
        {
            var manager = Make(s_SingleRegularDatabases);
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(1));
            Assert.That(manager.NonDynamicDatabaseName().Count(), Is.EqualTo(1));

            Assert.That(manager.HasDatabase("Island"), Is.True);
            Assert.That(manager.HasDatabase("ISLAND"), Is.True);
            Assert.That(manager.HasDatabase("island"), Is.True);
            Assert.That(manager.HasDatabase("foo"), Is.False);
            
            Assert.That(manager.IsDynamic("Island"), Is.False);
            Assert.That(manager.IsTransactional("Island"), Is.False);

            using(var connection = manager.OpenConnection("Island"))
            {
                Assert.That(connection, Is.Not.Null);
                Assert.That(connection.ConnectionString, Is.EqualTo("foo"));
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            }
        }

        [Test]
        public void MultipleDatabase()
        {
            var manager = Make(s_MultipleRegularDatabases);
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(2));
            Assert.That(manager.NonDynamicDatabaseName().Count(), Is.EqualTo(2));

            Assert.That(manager.HasDatabase("Island"), Is.True);
            Assert.That(manager.HasDatabase("ISLAND"), Is.True);
            Assert.That(manager.HasDatabase("island"), Is.True);
            Assert.That(manager.HasDatabase("London"), Is.True);
            Assert.That(manager.HasDatabase("LONDON"), Is.True);
            Assert.That(manager.HasDatabase("london"), Is.True);
            
            Assert.That(manager.IsDynamic("Island"), Is.False);
            Assert.That(manager.IsTransactional("Island"), Is.False);

            Assert.That(manager.IsDynamic("London"), Is.False);
            Assert.That(manager.IsTransactional("London"), Is.True);

            using(var connection = manager.OpenConnection("Island"))
            {
                Assert.That(connection, Is.Not.Null);
                Assert.That(connection.ConnectionString, Is.EqualTo("foo"));
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            }

            using(var connection = manager.OpenConnection("London"))
            {
                Assert.That(connection, Is.Not.Null);
                Assert.That(connection.ConnectionString, Is.EqualTo("bar"));
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            }
        }

        [Test]
        public void OpenDynamicDatabase_NonDynamically()
        {
            var manager = Make(s_SinglDynamicDatabases);
            Assert.Catch(() => manager.OpenConnection("Orchid"));
        }

        [Test]
        public void SingleDynamicDatabase()
        {
            var manager = Make(s_SinglDynamicDatabases);
            Assert.That(manager.DatabaseNames.Count(), Is.EqualTo(1));
            Assert.That(manager.NonDynamicDatabaseName().Count(), Is.EqualTo(0));

            Assert.That(manager.HasDatabase("Orchid"), Is.True);
            Assert.That(manager.HasDatabase("ORCHID"), Is.True);
            
            Assert.That(manager.IsDynamic("Orchid"), Is.True);
            Assert.That(manager.IsTransactional("Orchid"), Is.False);

            var arguments = new Dictionary<string, object>()
            {
                {"username", "Jack"}
            };

            using(var connection = manager.OpenDynamicConnection("Orchid", arguments))
            {
                Assert.That(connection, Is.Not.Null);
                Assert.That(connection.ConnectionString, Is.EqualTo("wibble-Jack"));
                Assert.That(connection.State, Is.EqualTo(ConnectionState.Open));
            }
        }

        private IDatabaseManager Make(Action<XmlDatabaseManager> init)
        {
            var manager = new XmlDatabaseManager();

            ISupportInitialize model = manager;
            model.BeginInit();
            init(manager);
            model.EndInit();

            return manager;
        }

        private IDatabaseManager Make(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var factory = InstanceFactory.New();
            var manager = factory.Create<XmlDatabaseManager>(doc.DocumentElement);

            return manager;
        }

        private static string s_SingleRegularDatabases =
        @"<DatabaseManager>
            <Databases>
                <Database name='Island'>
                    <Details type='UnitTests.Arrow.Data.DatabaseManagers.NullDatabaseDetails, UnitTests.Arrow'>
                        <ConnectionString>foo</ConnectionString>
                    </Details>
                </Database>
            </Databases>
        </DatabaseManager>  
        ";

        private static string s_MultipleRegularDatabases =
        @"<DatabaseManager>
            <Databases>
                <Database name='Island'>
                    <Details type='UnitTests.Arrow.Data.DatabaseManagers.NullDatabaseDetails, UnitTests.Arrow'>
                        <ConnectionString>foo</ConnectionString>
                    </Details>
                </Database>
                <Database name='London' transactional='true'>
                    <Details type='UnitTests.Arrow.Data.DatabaseManagers.NullDatabaseDetails, UnitTests.Arrow'>
                        <ConnectionString>bar</ConnectionString>
                    </Details>
                </Database>
            </Databases>
        </DatabaseManager> 
        ";

        private static string s_SinglDynamicDatabases =
        @"<DatabaseManager>
            <DynamicDatabases>
                <Database name='Orchid'>
                    <Details type='UnitTests.Arrow.Data.DatabaseManagers.NullDatabaseDetails, UnitTests.Arrow'>
                        <ConnectionString>wibble-${username}</ConnectionString>
                    </Details>
                </Database>
            </DynamicDatabases>
        </DatabaseManager>  
        ";

        private static string s_DuplicateRegularDatabases =
        @"<DatabaseManager>
            <Databases>
                <Database name='Island'>
                    <Details type='UnitTests.Arrow.Data.DatabaseManagers.NullDatabaseDetails, UnitTests.Arrow'>
                        <ConnectionString>foo</ConnectionString>
                    </Details>
                </Database>
                <Database name='island' transactional='true'>
                    <Details type='UnitTests.Arrow.Data.DatabaseManagers.NullDatabaseDetails, UnitTests.Arrow'>
                        <ConnectionString>foo</ConnectionString>
                    </Details>
                </Database>
            </Databases>
        </DatabaseManager>  
        ";

        private static string s_DuplicateDynamicDatabases =
        @"<DatabaseManager>
            <DynamicDatabases>
                <Database name='Orchid'>
                    <Details type='UnitTests.Arrow.Data.DatabaseManagers.NullDatabaseDetails, UnitTests.Arrow'>
                        <ConnectionString>foo</ConnectionString>
                    </Details>
                </Database>
                <Database name='ORCHID'>
                    <Details type='UnitTests.Arrow.Data.DatabaseManagers.NullDatabaseDetails, UnitTests.Arrow'>
                        <ConnectionString>foo</ConnectionString>
                    </Details>
                </Database>
            </DynamicDatabases>
        </DatabaseManager>  
        ";
    }
}
