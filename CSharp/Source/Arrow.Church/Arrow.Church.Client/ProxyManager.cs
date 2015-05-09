using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Client.Proxy;

namespace Arrow.Church.Client
{
	public static class ProxyManager
	{		
		public static ProxyFactory FactoryFor(Type @interface)
		{
			if(@interface==null) throw new ArgumentNullException("@interface");
			if(@interface.IsInterface==false) throw new ArgumentException("not an interface: "+@interface.Name,"@interface");

			var creator=ProxyBase.GetProxyCreator(@interface);
			return new ProxyFactory(creator,@interface);
		}

		public static ProxyFactory<T> FactoryFor<T>() where T:class
		{
			if(typeof(T).IsInterface==false) throw new ArgumentException("generic type not an interface: "+typeof(T).Name);

			var creator=ProxyBase.GetProxyCreator(typeof(T));
			return new ProxyFactory<T>(creator);
		}
	}
}
