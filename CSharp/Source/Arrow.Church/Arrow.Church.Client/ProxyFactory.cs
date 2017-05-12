using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Client
{
	public class ProxyFactory
	{
		private readonly ProxyCreator m_ProxyCreator;
		private readonly Type m_ServiceInterface;

		internal ProxyFactory(ProxyCreator proxyCreator, Type serviceInterface)
		{
			m_ProxyCreator=proxyCreator;
			m_ServiceInterface=serviceInterface;
		}

		public TInterface Create<TInterface>(string serviceName, Uri endpoint) where TInterface:class
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");
			if(endpoint.AbsolutePath!="/") throw new ArgumentException("endpoint contains a service name when one has been explicitly supplied","endpoint");
			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException("service name is whitespace","serviceName");
			if(typeof(TInterface).IsAssignableFrom(m_ServiceInterface)==false) throw new ArgumentException("generic type not compatible with service interface");

			var instance=m_ProxyCreator(endpoint,serviceName);
			return (TInterface)instance;
		}

		public TInterface Create<TInterface>(Uri endpoint) where TInterface:class
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");

			var builder=new UriBuilder(endpoint);
			
			string serviceName=builder.Path;
			if(serviceName.StartsWith("/")) serviceName=serviceName.Substring(1);
			
			// We dont need the path any more
			builder.Path="";

			var adjustedEndpoint=builder.Uri;
			return Create<TInterface>(serviceName,adjustedEndpoint);
		}
	}

	public class ProxyFactory<TInterface> : ProxyFactory where TInterface:class
	{
		internal ProxyFactory(ProxyCreator proxyCreator) : base(proxyCreator,typeof(TInterface))
		{
		}

		public TInterface Create(string serviceName, Uri endpoint)
		{
			return Create<TInterface>(serviceName,endpoint);
		}

		public TInterface Create(Uri endpoint)
		{
			return Create<TInterface>(endpoint);
		}
	}
}
