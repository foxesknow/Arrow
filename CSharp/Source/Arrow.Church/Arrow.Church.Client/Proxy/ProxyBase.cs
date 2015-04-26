using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Client.Proxy
{
	public abstract partial class ProxyBase
	{
		private readonly ServiceDispatcher m_ServiceDispatcher;
		private readonly string m_ServiceName;

		public ProxyBase(ServiceDispatcher serviceDispatcher, string serviceName)
		{
			if(serviceDispatcher==null) throw new ArgumentNullException("serviceDispatcher");
			if(serviceName==null) throw new ArgumentNullException("serviceName");

			m_ServiceDispatcher=serviceDispatcher;
			m_ServiceName=serviceName;
		}

		protected Task<T> GenericCall<T>(string methodName, object request)
		{
			return m_ServiceDispatcher.Call<T>(m_ServiceName,methodName,request);
		}

		protected Task Call(string methodName, object request)
		{
			return m_ServiceDispatcher.Call(m_ServiceName,methodName,request);
		}
	}
}
