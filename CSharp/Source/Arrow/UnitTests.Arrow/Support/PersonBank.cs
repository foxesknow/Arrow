using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Arrow.Support
{
	public class PersonBank
	{
		private Person m_Primary;
		private Person m_Secondary;
		
		public Person Primary
		{
			get{return m_Primary;}
			set{m_Primary = value;}
		}
		
		public Person Secondary
		{
			get{return m_Secondary;}
			set{m_Secondary = value;}
		}
	}
}
