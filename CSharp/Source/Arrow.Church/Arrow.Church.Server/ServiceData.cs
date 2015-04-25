using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;

namespace Arrow.Church.Server
{
	class ServiceData
	{
		private readonly ChurchServiceBase m_Service;
		private readonly Dictionary<string,ServiceMethod> m_ServiceMethods=new Dictionary<string,ServiceMethod>();
		private readonly HashSet<Type> m_Interfaces=new HashSet<Type>();

		public ServiceData(ChurchServiceBase service)
		{
			m_Service=service;
		}

		public ChurchServiceBase Service
		{
			get{return m_Service;}
		}

		public void AddMethod(string name, ServiceMethod method)
		{
			m_ServiceMethods.Add(name,method);
		}

		public void AddInterface(Type type)
		{
			m_Interfaces.Add(type);
		}

		public bool HasInterface(Type type)
		{
			return m_Interfaces.Contains(type);
		}

		public Task<object> Execute(string method, object argument)
		{
			ServiceMethod serviceMethod=null;

			if(m_ServiceMethods.TryGetValue(method,out serviceMethod))
			{
				return serviceMethod(m_Service,argument);
			}
			else
			{
				throw new ChurchException("could not find method: "+method);
			}
		}
	}
}
