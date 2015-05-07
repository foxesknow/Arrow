using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Client.Proxy
{
	public sealed class ProxyFactory
	{
		private readonly ProxyCreator m_ProxyCreator;

		internal ProxyFactory(ProxyCreator proxyCreator)
		{
			m_ProxyCreator=proxyCreator;
		}

		public T Create<T>(Uri endpoint, string serviceName) where T:class
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");
			if(endpoint.AbsolutePath!="/") throw new ArgumentException("endpoint contains a service name when one has been explicitly supplied","endpoint");
			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException("service name is whitespace","serviceName");

			var instance=m_ProxyCreator(endpoint,serviceName);
			return (T)instance;
		}

		public T Create<T>(Uri endpoint) where T:class
		{
			if(endpoint==null) throw new ArgumentNullException("endpoint");

			var builder=new UriBuilder(endpoint);
			
			string serviceName=builder.Path;
			if(serviceName.StartsWith("/")) serviceName=serviceName.Substring(1);
			
			// We dont need the path any more
			builder.Path="";

			var adjustedEndpoint=builder.Uri;
			return Create<T>(adjustedEndpoint,serviceName);
		}
	}
}
