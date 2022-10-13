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
	public class EnvironmentSettingsTests
	{
		[Test]
		public void Test()
		{
			ISettings p=CreateProvider();
			
			object value=null;
			value=p.GetSetting("username");
			Assert.IsNotNull(value);
			Assert.That(value.ToString().Length,Is.GreaterThan(0));
			
			value=p.GetSetting("foo");
			Assert.IsNull(value);
		}
		
		internal static ISettings CreateProvider()
		{
			return EnvironmentSettings.Instance;
		}
	}
}
