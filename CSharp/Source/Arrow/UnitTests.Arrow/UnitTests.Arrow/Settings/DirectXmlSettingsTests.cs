using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Settings;
using Arrow.Text;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Settings
{
	[TestFixture]
	public class DirectXmlSettingsTests
	{
		[Test]
		public void Test()
		{
			string name=TokenExpander.ExpandText("${Person:Name}");
			Assert.That(name,Is.EqualTo("Sawyer"));
		}
	}
}
