using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Client.ServiceDispatchers;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Client.Proxy
{
	/// <summary>
	/// The base class for all proxies
	/// </summary>
	public abstract partial class ProxyBase
	{
		private readonly ServiceDispatcher m_ServiceDispatcher;
		private readonly string m_ServiceName;
		private readonly MessageProtocol m_MessageProtocol;

		private readonly Dictionary<string,Type> m_MethodReturnTypes=new Dictionary<string,Type>();

		protected ProxyBase(Uri endpoint, string serviceName, MessageProtocol messageProtocol)
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");
			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(messageProtocol==null) throw new ArgumentNullException("messageProtocol");

			m_ServiceDispatcher=ServiceDispatcherManager.GetServiceDispatcher(endpoint);
			m_ServiceName=serviceName;
			m_MessageProtocol=messageProtocol;
		}

		/// <summary>
		/// Records the return type for a method.
		/// The outer Task has been stripped from the return type.
		/// This is used by the framework to deserialize a response from the service
		/// </summary>
		/// <param name="methodName">The name of the method</param>
		/// <param name="returnType">The type of data it returns</param>
		protected void AddReturnType(string methodName, Type returnType)
		{
			m_MethodReturnTypes.Add(methodName,returnType);
		}

		internal bool TryGetReturnType(string methodName, out Type returnType)
		{
			return m_MethodReturnTypes.TryGetValue(methodName,out returnType);
		}

		/// <summary>
		/// Called by the codegen layer when a method returning a generic Task is called
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="methodName">The name of the method to call</param>
		/// <param name="request"></param>
		/// <returns></returns>
		protected Task<T> GenericCall<T>(string methodName, object request)
		{
			return m_ServiceDispatcher.Call<T>(this,m_ServiceName,methodName,request);
		}

		/// <summary>
		/// Called by the codegen layer when a method returning a non-generic task is called
		/// </summary>
		/// <param name="methodName">The name of the method to call</param>
		/// <param name="request"></param>
		/// <returns></returns>
		protected Task Call(string methodName, object request)
		{
			return m_ServiceDispatcher.Call(this,m_ServiceName,methodName,request);
		}

		/// <summary>
		/// The encoding protocol used by the service we're proxying to
		/// </summary>
		protected internal MessageProtocol MessageProtocol
		{
			get{return m_MessageProtocol;}
		}

		public override string ToString()
		{
			return string.Format("Service={0}, Endpoint={1}",m_ServiceName,m_ServiceDispatcher.Endpoint);
		}
	}
}
