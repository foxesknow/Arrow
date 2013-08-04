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
			m_ConfigFile=AppConfig.ConfigFile;
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
			
			XmlNode node=AppConfig.GetSectionXml(ArrowSystem.Name,"Data");
			Assert.IsNull(node);
		}
		
		[Test]
		public void StorageConfig()
		{
			Uri uri=ResourceLoader.MakeUri("AppConfigTest_Storage.xml");
			IConfigFile configFile=new StorageConfigFile(uri);
			AppConfig.ReplaceConfig(configFile);
			
			string superuser=AppConfig.GetSectionObject<string>(ArrowSystem.Name,"Users/Superuser");
			Assert.IsNotNull(superuser);
			Assert.That(superuser,Is.EqualTo("Ben"));
			
			XmlNode node=AppConfig.GetSectionXml(ArrowSystem.Name,"Users/Superuser");
			Assert.IsNotNull(node);
			Assert.That(node.Name,Is.EqualTo("Superuser"));
		}
		
		[Test]
		public void ApplicationConfig()
		{
			XmlNode node=AppConfig.GetSectionXml(ArrowSystem.Name,"UnitTests/TestString");
			Assert.IsNotNull(node);
			Assert.That(node.InnerText,Is.EqualTo("hello"));
		}
		
		[Test]
		public void MissingUriResource()
		{
			XmlNode node=AppConfig.GetSectionXml(ArrowSystem.Name,"MissingUnitTests");
			Assert.IsNull(node);
		}
		
		[Test]
		[ExpectedException(typeof(System.IO.IOException))]
		public void MandatoryUriResource()
		{
			XmlNode node=AppConfig.GetSectionXml(ArrowSystem.Name,"MandatoryResource");
			Assert.IsNull(node);
		}
		
		[Test]
		public void GetSystemXml()
		{
			XmlNode node=AppConfig.GetSystemXml(ArrowSystem.Name);
			Assert.IsNotNull(node);
			Assert.That(node.ChildNodes.Count,Is.Not.EqualTo(0));
		}
		
		[Test]
		public void LegacyGetConfig()
		{
			NameValueCollection values=AppConfig.LegacyGetConfig("OldStyle");
			Assert.IsNotNull(values);
			
			Assert.That(values.Count,Is.EqualTo(3));
			Assert.That(values["Jack"],Is.EqualTo("Doctor"));
			Assert.That(values["Sawyer"],Is.EqualTo("Conman"));
			
			// We should have a value guid as the settings have been expanded
			string unique=values["Unique"];
			Guid guid=new Guid(unique);
		}
	}
}
