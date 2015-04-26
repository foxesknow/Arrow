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

		public ProxyBase(ServiceDispatcher serviceDispatcher)
		{
			if(serviceDispatcher==null) throw new ArgumentNullException("serviceDispatcher");

			m_ServiceDispatcher=serviceDispatcher;
		}

		protected Task<T> GenericCall<T>(string methodName, object request)
		{
			return m_ServiceDispatcher.Call<T>(methodName,request);
		}

		protected Task Call(string methodName, object request)
		{
			return m_ServiceDispatcher.Call(methodName,request);
		}
	}
}
