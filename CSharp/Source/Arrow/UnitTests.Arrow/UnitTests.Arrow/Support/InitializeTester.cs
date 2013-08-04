using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using NUnit.Framework;

namespace UnitTests.Arrow.Support
{
	public class InitializeTester : ISupportInitialize
	{
		internal bool m_BeginInitCalled;
		internal bool m_EndInitCalled;
		
		private int m_Age;
		
		public int Age
		{
			get{return m_Age;}
			set
			{
				Assert.IsTrue(m_BeginInitCalled);
				Assert.IsFalse(m_EndInitCalled);
				m_Age=value;
			}
		}

		void ISupportInitialize.BeginInit()
		{
			Assert.IsFalse(m_BeginInitCalled);
			Assert.IsFalse(m_EndInitCalled);
			
			m_BeginInitCalled=true;
		}

		void ISupportInitialize.EndInit()
		{
			Assert.IsTrue(m_BeginInitCalled);
			Assert.IsFalse(m_EndInitCalled);
			
			m_EndInitCalled=true;
		}
	}
}
