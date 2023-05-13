using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTests.Arrow.Support
{
	public class Foo
    {
        private string m_Username;
        private bool m_Enabled;

        private int? m_Offset;
        private bool? m_Allow = true;
        private IBasic m_Basic;
        private DateTime m_When;

        private List<int> m_Numbers = new List<int>();
        private Dictionary<string, IBasic> m_Basics = new Dictionary<string, IBasic>();

        private Dictionary<string, int> m_Ages = new Dictionary<string, int>();

        public Foo(string username, bool enabled)
        {
            m_Username = username;
            m_Enabled = enabled;
        }

        public string Username
		{
			get{return m_Username;}
		}
		
		public bool Enabled
		{
			get{return m_Enabled;}
		}
		
		public IBasic Basic
		{
			get{return m_Basic;}
			set{m_Basic = value;}
		}
		
		public DateTime When
		{
			get{return m_When;}
			set{m_When = value;}
		}
		
		public bool? Allow
		{
			get{return m_Allow;}
			set{m_Allow = value;}
		}
		
		public int? Offset
		{
			get{return m_Offset;}
			set{m_Offset = value;}
		}
		
		public IList<int> Numbers
		{
			get{return m_Numbers;}
		}
		
		public IDictionary<string,IBasic> Basics
		{
			get{return m_Basics;}
		}
		
		public Dictionary<string,int> Ages
		{
			get{return m_Ages;}
		}
	}
}
