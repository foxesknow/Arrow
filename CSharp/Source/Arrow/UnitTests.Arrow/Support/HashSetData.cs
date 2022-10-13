using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Arrow.Support
{
	class HashSetData
	{
		private HashSet<int> m_Data=new HashSet<int>();
		
		public HashSet<int> Data
		{
			get{return m_Data;}
		}
	}
}
