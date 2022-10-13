using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Settings;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Settings
{
	[TestFixture]
	public class SettingsManagerTests
	{
		[Test]
		public void TestSettingAccess()
		{
			DateTime now=SettingsManager.Setting<DateTime>("datetime:now");
			Assert.That(SettingsManager.TryGetSetting<DateTime>("datetime:now",out now),Is.True);
			
			// proc:pid is a string. This will test type conversion
			int pid=SettingsManager.Setting<int>("proc:pid");
			Assert.IsTrue(pid!=0);
			Assert.IsTrue(SettingsManager.TryGetSetting<int>("proc:pid",out pid));
			
			string d=SettingsManager.Setting<string>("foo:bar","default");
			Assert.IsNotNull(d);
			Assert.IsTrue(d=="default");
			Assert.IsFalse(SettingsManager.TryGetSetting<string>("foo:bar",out d));
		}
		
		[Test]
		public void TestProviderLookup()
		{
			Assert.IsNotNull(SettingsManager.GetSettings("proc"));
			Assert.IsNull(SettingsManager.GetSettings("foo"));
		}
		
		[Test]
		public void TestNamespaces()
		{
			List<string> n=SettingsManager.Namespaces;
			Assert.IsNotNull(n);
			Assert.IsNotEmpty(n);
		
			Assert.Contains("datetime",n);
			Assert.Contains("proc",n);
			Assert.IsFalse(n.Contains("foo"));
		}
	}
}
