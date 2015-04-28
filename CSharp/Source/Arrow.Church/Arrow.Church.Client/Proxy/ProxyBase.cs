using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Client.Proxy
{
	public abstract partial class ProxyBase
	{
		private readonly ServiceDispatcher m_ServiceDispatcher;
		private readonly string m_ServiceName;
		private readonly MessageProtocol m_MessageProtocol;

		public ProxyBase(ServiceDispatcher serviceDispatcher, string serviceName, MessageProtocol messageProtocol)
		{
			if(serviceDispatcher==null) throw new ArgumentNullException("serviceDispatcher");
			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(messageProtocol==null) throw new ArgumentNullException("messageProtocol");

			m_ServiceDispatcher=serviceDispatcher;
			m_ServiceName=serviceName;
			m_MessageProtocol=messageProtocol;
		}

		protected Task<T> GenericCall<T>(string methodName, object request)
		{
			return m_ServiceDispatcher.Call<T>(this,m_ServiceName,methodName,request);
		}

		protected Task Call(string methodName, object request)
		{
			return m_ServiceDispatcher.Call(this,m_ServiceName,methodName,request);
		}

		protected internal MessageProtocol MessageProtocol
		{
			get{return m_MessageProtocol;}
		}
	}
}
