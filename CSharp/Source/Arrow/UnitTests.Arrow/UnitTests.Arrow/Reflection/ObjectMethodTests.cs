using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Reflection;

using NUnit.Framework;
using NUnit.Framework.Constraints;

using UnitTests.Arrow.Support;

namespace UnitTests.Arrow.Reflection
{
	[TestFixture]
	public class ObjectMethodTests
	{
		[Test]
		public void Test_Equals()
		{
			var p1=new Person(){Name="Jack", Age=40};
			var p2=new Person(){Name="Jack", Age=40};

			var eq=ObjectMethod.MakeEquals<Person>();
			Assert.That(eq,Is.Not.Null);

			Assert.That(eq(p1,p1), Is.True);
			Assert.That(eq(p1,p2), Is.True);
		}

		[Test]
		public void Test_NotEquals()
		{
			var p1=new Person(){Name="Jack", Age=40};
			var p2=new Person(){Name="Sawyer", Age=40};

			var eq=ObjectMethod.MakeEquals<Person>();
			Assert.That(eq,Is.Not.Null);

			Assert.That(eq(p1,p2), Is.False);
			Assert.That(eq(p1,null), Is.False);
		}

		[Test]
		public void Test_Equals_AllCalled()
		{
			var r1=new Recorder(){Name="Jack", Age=40, Location="Island"};
			var r2=new Recorder(){Name="Jack", Age=40, Location="Island"};

			var eq=ObjectMethod.MakeEquals<Recorder>();
			Assert.That(eq,Is.Not.Null);

			Assert.That(eq(r1,r2), Is.True);
			Assert.That(r1.AllTouched(), Is.True);
			Assert.That(r2.AllTouched(), Is.True);
		}

		[Test]
		public void Test_Equals_NotAllCalled()
		{
			var r1=new Recorder(){Name="Jack", Age=40, Location="Island"};
			var r2=new Recorder(){Name="Jack", Age=41, Location="Mainland"};

			var eq=ObjectMethod.MakeEquals<Recorder>();
			Assert.That(eq,Is.Not.Null);

			Assert.That(eq(r1,r2), Is.False);
			Assert.That(r1.AllTouched(), Is.False);
			Assert.That(r2.AllTouched(), Is.False);
		}

		[Test]
		public void Test_GetHashCode()
		{
			var p1=new Person(){Name="Jack", Age=40};
			var p2=new Person(){Name="Sawyer", Age=40};

			var others=new TheOthers(){Leader=p1, Medic=p2};

			var hasher=ObjectMethod.MakeGetHashCode<TheOthers>();
			Assert.That(hasher,Is.Not.Null);
			
			// There's not much we can test here, as the value could be anything!
			int hash=hasher(others);
		}

		[Test]
		public void Test_GetHashCode_AllCalled()
		{
			var r1=new Recorder(){Name="Jack", Age=40, Location="Island"};

			var hasher=ObjectMethod.MakeGetHashCode<Recorder>();
			Assert.That(hasher,Is.Not.Null);

			int hash=hasher(r1);
			Assert.That(r1.AllTouched(), Is.True);
		}

		[Test]
		public void Test_ToString()
		{
			var p1=new Person(){Name="Jack", Age=40};

			var tostring=ObjectMethod.MakeToString<Person>();
			Assert.That(tostring,Is.Not.Null);
			
			var text=tostring(p1);
			Assert.That(text,Is.Not.Null);
			Assert.That(text,Has.Length.GreaterThan(0));
		}

		[Test]
		public void Test_ToString_AllCalled()
		{
			var r1=new Recorder(){Name="Jack", Age=40, Location="Island"};

			var tostring=ObjectMethod.MakeToString<Recorder>();
			Assert.That(tostring,Is.Not.Null);

			var text=tostring(r1);
			Assert.That(r1.AllTouched(), Is.True);
		}

		class Recorder
		{
			private string m_Name;
			private bool m_NameTouched;

			private int m_Age;
			private bool m_AgeTouched;

			private string m_Location;
			private bool m_LocationTouched;

			public string Name
			{
				get{m_NameTouched=true; return m_Name;}
				set{m_Name=value;}
			}

			public string Location
			{
				get{m_LocationTouched=true; return m_Location;}
				set{m_Location=value;}
			}

			public int Age
			{
				get{m_AgeTouched=true; return m_Age;}
				set{m_Age=value;}
			}

			public bool AllTouched()
			{
				return m_NameTouched && m_AgeTouched && m_LocationTouched;
			}
		}
	}
}
