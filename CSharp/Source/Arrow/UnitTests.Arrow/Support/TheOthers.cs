using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Arrow.Support
{
	public class TheOthers
	{
        private Person m_Leader = new Person();
        private Person m_Medic = new Person();

        private Person m_OriginalMedic;
		
		public TheOthers()
		{
            m_Medic.Name = "Ethan";
            m_Medic.Age = 38;
            m_OriginalMedic = m_Medic;
        }
		
		public Person Leader
		{
			get{return m_Leader;}
			set{m_Leader = value;}
		}
		
		public Person Medic
		{
			get{return m_Medic;}
			set{m_Medic = value;}
		}
		
		public Person OriginalMedic
		{
			get{return m_OriginalMedic;}
		}
	}
}
