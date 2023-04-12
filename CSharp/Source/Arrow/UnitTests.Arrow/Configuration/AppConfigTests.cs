using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections.Specialized;

using Arrow.Configuration;

using NUnit.Framework;

namespace UnitTests.Arrow.Configuration
{
    [TestFixture]
    public class AppConfigTests
    {
        private IConfigFile m_ConfigFile;

        [SetUp]
        public void Init()
        {
            // Make sure we always revert the config system
            // back to how it was before each test ran
            m_ConfigFile = AppConfig.ConfigFile;
        }

        [TearDown]
        public void TidyUp()
        {
            AppConfig.ReplaceConfig(m_ConfigFile);
        }

        [Test]
        public void NullConfig()
        {
            AppConfig.ReplaceConfig(new NullConfigFile());

            XmlNode node = AppConfig.GetSectionXml(ArrowSystem.Name, "Data");
            Assert.IsNull(node);
        }

        [Test]
        public void StorageConfig()
        {
            Uri uri = ResourceLoader.MakeUri("AppConfigTest_Storage.xml");
            IConfigFile configFile = new StorageConfigFile(uri);
            AppConfig.ReplaceConfig(configFile);

            string superuser = AppConfig.GetSectionObject<string>(ArrowSystem.Name, "Users/Superuser");
            Assert.IsNotNull(superuser);
            Assert.That(superuser, Is.EqualTo("Ben"));

            XmlNode node = AppConfig.GetSectionXml(ArrowSystem.Name, "Users/Superuser");
            Assert.IsNotNull(node);
            Assert.That(node.Name, Is.EqualTo("Superuser"));
        }

        [Test]
        public void ApplicationConfig()
        {
            XmlNode node = AppConfig.GetSectionXml(ArrowSystem.Name, "UnitTests/TestString");
            Assert.IsNotNull(node);
            Assert.That(node.InnerText, Is.EqualTo("hello"));
        }

        [Test]
        public void MissingUriResource()
        {
            XmlNode node = AppConfig.GetSectionXml(ArrowSystem.Name, "MissingUnitTests");
            Assert.IsNull(node);
        }

        [Test]
        public void MandatoryUriResource()
        {
            Assert.Throws<System.IO.IOException>(() =>
            {
                XmlNode node = AppConfig.GetSectionXml(ArrowSystem.Name, "MandatoryResource");
                Assert.IsNull(node);
            });
        }

        [Test]
        public void GetSystemXml()
        {
            XmlNode node = AppConfig.GetSystemXml(ArrowSystem.Name);
            Assert.IsNotNull(node);
            Assert.That(node.ChildNodes.Count, Is.Not.EqualTo(0));
        }

        [Test]
        public void LegacyGetConfig()
        {
            NameValueCollection values = AppConfig.LegacyGetConfig("OldStyle");
            Assert.IsNotNull(values);

            Assert.That(values.Count, Is.EqualTo(3));
            Assert.That(values["Jack"], Is.EqualTo("Doctor"));
            Assert.That(values["Sawyer"], Is.EqualTo("Conman"));

            // We should have a value guid as the settings have been expanded
            string unique = values["Unique"];
            Guid guid = new Guid(unique);
        }

        [Test]
        public void LegacyGetConfig_AppSettings()
        {
            NameValueCollection values = AppConfig.LegacyGetConfig("appSettings");
            Assert.That(values, Is.Not.Null);

            var whereToWrite = values["WhereToWrite"];
            Assert.That(whereToWrite, Is.Not.Null & Has.Length.GreaterThan(0));

            // Variable expansion SHOULD have occured
            Assert.That(whereToWrite, Is.Not.EqualTo("${env:temp}"));
        }

        [Test]
        public void ConnectionStrings()
        {
            var settings = AppConfig.ConnectionStrings;

            var prod = settings["Prod"];
            Assert.That(prod, Is.Not.Null);
            Assert.That(prod.Name, Is.EqualTo("Prod"));
            Assert.That(prod.ProviderName, Is.EqualTo("Null"));
            Assert.That(prod.ConnectionString, Is.EqualTo("Island"));

            var uat = settings["Uat"];
            Assert.That(uat, Is.Not.Null);
            Assert.That(uat.Name, Is.EqualTo("Uat"));
            Assert.That(uat.ProviderName, Is.EqualTo("Oracle"));
            Assert.That(uat.ConnectionString, Is.EqualTo("Orchid"));
        }

        [Test]
        public void AppSettings()
        {
            var appSettings = AppConfig.AppSettings;
            Assert.That(appSettings, Is.Not.Null);

            // Variable expansion should NOT have happened
            var value = appSettings["WhereToWrite"];
            Assert.That(value, Is.Not.Null & Is.EqualTo("${env:temp}"));

            // We should get the same collection each call
            Assert.That(appSettings, Is.SameAs(AppConfig.AppSettings));

            // The includes should have been processed
            var location = appSettings["Location"];
            Assert.That(location, Is.EqualTo("Island"));

            // This should have loaded even though the filename comes via an ${appSetting}
            var loopedName = appSettings["LoopedName"];
            Assert.That(loopedName, Is.EqualTo("Jack"));

            // This should have been removed
            var toDelete = appSettings["ToDelete"];
            Assert.That(toDelete, Is.Null);
        }

        [Test]
        public void LazyReload()
        {
            var appSettings = AppConfig.AppSettings;
            var connectionStrings = AppConfig.ConnectionStrings;

            var configFile = new XmlTextConfigFile(@"
				<configuration>
					<appSettings>
						<add key='Who' value='Jack' />
					</appSettings>
					<connectionStrings>
						<add name='Dev' connectionString='Flame' providerName='Sybase' />
					</connectionStrings>
				</configuration>
			");

            AppConfig.ReplaceConfig(configFile);

            var newAppSetting = AppConfig.AppSettings;
            Assert.That(newAppSetting, Is.Not.SameAs(appSettings));
            Assert.That(newAppSetting.Count, Is.EqualTo(1));
            Assert.That(newAppSetting["who"], Is.EqualTo("Jack"));

            var newConnectionStrings = AppConfig.ConnectionStrings;
            Assert.That(newConnectionStrings, Is.Not.SameAs(connectionStrings));
            Assert.That(newConnectionStrings.Count, Is.EqualTo(1));
            Assert.That(newConnectionStrings["dev"].ConnectionString, Is.EqualTo("Flame"));
            Assert.That(newConnectionStrings["dev"].ProviderName, Is.EqualTo("Sybase"));
        }
    }
}
