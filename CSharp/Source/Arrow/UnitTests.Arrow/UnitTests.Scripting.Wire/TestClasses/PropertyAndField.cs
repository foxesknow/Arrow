using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Scripting.Wire.TestClasses
{
	class PropertyAndField
	{
		private static readonly List<string> SomeNames=new List<string>(){"Dom","Rob","John"};

		public static readonly int sNumber=58;
		public static readonly string sText="HelloWorld";

		public static int StaticNumber
		{
			get{return 101;}
		}

		public static IList<string> StaticNames
		{
			get{return SomeNames;}
		}

		public int Counter
		{
			get{return 42;}
		}

		public string Leader
		{
			get{return "Ben";}
		}

		public static string NullString
		{
			get{return null;}
		}
	}
}
