using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Arrow.Support
{
	public interface IBasic
	{
		string GetName();
	}

	public class BasicFoo : IBasic
	{
		public string GetName()
		{
			return "BasicFoo";
		}
	}

	public class BasicBar : IBasic
	{
		public string GetName()
		{
			return "BasicBar";
		}
	}
}
