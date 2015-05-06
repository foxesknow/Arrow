using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Client.ServiceDispatchers;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Client.Proxy
{
	public abstract partial class ProxyBase
	{
		private readonly ServiceDispatcher m_ServiceDispatcher;
		private readonly string m_ServiceName;
		private readonly MessageProtocol m_MessageProtocol;

		private readonly Dictionary<string,Type> m_MethodReturnTypes=new Dictionary<string,Type>();

		public ProxyBase(Uri endpoint, string serviceName, MessageProtocol messageProtocol)
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");
			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(messageProtocol==null) throw new ArgumentNullException("messageProtocol");

			m_ServiceDispatcher=ServiceDispatcherManager.GetServiceDispatcher(endpoint);
			m_ServiceName=serviceName;
			m_MessageProtocol=messageProtocol;
		}

		protected void AddReturnType(string methodName, Type returnType)
		{
			m_MethodReturnTypes.Add(methodName,returnType);
		}

		internal bool TryGetReturnType(string methodName, out Type returnType)
		{
			return m_MethodReturnTypes.TryGetValue(methodName,out returnType);
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
