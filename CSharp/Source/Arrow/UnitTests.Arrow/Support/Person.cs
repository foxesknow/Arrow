using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Arrow.Support
{
	public class Person
	{
		private string m_Name;
		private int m_Age;
		
		public Person()
		{
		}
		
		public string Name
		{
			get{return m_Name;}
			set{m_Name = value;}
		}
		
		public int Age
		{
			get{return m_Age;}
			set{m_Age = value;}
		}
		
		public void InitializeAge(int age)
		{
			m_Age = age;
		}
	}
}
