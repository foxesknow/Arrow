using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Text;

using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace UnitTests.Arrow.Text
{
	[TestFixture]
	public class TokenExpanderTests
	{
		[Test]
		public void TestExpandText()
		{
			string text="hello there {env:username} the date is {datetime:now||yyyyMMdd}";
			string expanded=TokenExpander.ExpandText(text,"{","}");
			
			Assert.IsTrue(expanded!=null);
			Assert.IsTrue(expanded.Length!=0);
		}
		
		[Test]
		public void TestExpandTextLookup()
		{
			string text="hello there ${test:username|||toupper}$ your id is ${test:id}$";
			string expanded=TokenExpander.ExpandText(text,"${","}$",(string name)=>
			{
				if(name=="test:username") return "sean";
				if(name=="test:id") return "1";
				throw new ApplicationException();
			});
			
			Assert.IsTrue(expanded!=null);
			Assert.IsTrue(expanded=="hello there SEAN your id is 1");
		}
		
		[Test]
		public void TestPropertyLookup()
		{
			string text="test:username|Length";
			string expanded=TokenExpander.ExpandToken(text,(string name)=>
			{
				if(name=="test:username") return "sean";
				if(name=="test:id") return "1";
				throw new ApplicationException();
			});
			
			Assert.That(expanded,Is.EqualTo("4"));
		}
		
		[Test]
		public void TestDoublePropertyLookup()
		{
			DateTime when=new DateTime(2008,12,7); // A sunday
			
			string text="when|Date.DayOfWeek";
			string expanded=TokenExpander.ExpandToken(text,(string name)=>
			{
				if(name=="when") return when;
				throw new ApplicationException();
			});
			
			Assert.That(expanded,Is.EqualTo("Sunday"));
		}
	}
}
