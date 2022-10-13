using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace UnitTests.Arrow.Support
{
	public class XmlAssignment
	{
		private int m_Age;
		private XmlNode m_Config;
		
		public int Age
		{
			get{return m_Age;}
			set{m_Age=value;}
		}
		
		public XmlNode Config
		{
			get{return m_Config;}
			set{m_Config=value;}
		}
	}
}
