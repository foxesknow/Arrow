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
	public class DefaultSettingsTests
	{
		[Test]
		public void UnquotedUnmatched()
		{
			string text="hello there {def:Bob:foo:username}";
			string expanded=TokenExpander.ExpandText(text,"{","}");
			
			Assert.IsNotNull(expanded);
			Assert.That(expanded,Is.EqualTo("hello there Bob"));
		}
		
		[Test]
		public void QuotedUnmatched()
		{
			string text="hello there {def:\"Bob\":foo:username}";
			string expanded=TokenExpander.ExpandText(text,"{","}");
			
			Assert.IsNotNull(expanded);
			Assert.That(expanded,Is.EqualTo("hello there Bob"));
		}
		
		[Test]
		public void UnquotedMatched()
		{
			string text="hello there {def:Bob:env:username}";
			string expanded=TokenExpander.ExpandText(text,"{","}");
			
			Assert.IsNotNull(expanded);
			Assert.That(expanded,Is.Not.EqualTo("hello there Bob"));
		}
		
		[Test]
		public void QuotedMatched()
		{
			string text="hello there {def:\"Bob\":env:username}";
			string expanded=TokenExpander.ExpandText(text,"{","}");
			
			Assert.IsNotNull(expanded);
			Assert.That(expanded,Is.Not.EqualTo("hello there Bob"));
		}
	}
}
