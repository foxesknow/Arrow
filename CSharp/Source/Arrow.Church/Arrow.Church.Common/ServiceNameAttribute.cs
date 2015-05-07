using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common
{
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=false)]
	public sealed class ServiceNameAttribute : Attribute
	{
		private readonly string m_Name;

		public ServiceNameAttribute(string name)
		{
			if(name==null) throw new ArgumentNullException("name");
			if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty",name);

			m_Name=name;
		}

		public string Name
		{
			get{return m_Name;}
		}
	}
}
