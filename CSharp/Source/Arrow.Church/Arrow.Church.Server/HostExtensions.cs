using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common;

namespace Arrow.Church.Server
{
	public static class HostExtensions
	{
		/// <summary>
		/// Looks for a serice implementing the specified service interface.
		/// If zero, or more than one, are found then an exception is thrown
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="host"></param>
		/// <returns></returns>
		public static TService Discover<TService>(this IHost host) where TService:class
		{
			var services=host.DiscoverAll<TService>();

			if(services.Count==1)
			{
				return services[0];
			}
			else if(services.Count==0)
			{
				throw new ServiceNotFoundException("could not find a service implementing "+typeof(TService).Name);
			}
			else
			{
				throw new ChurchException("found multiple matching services for "+typeof(TService).Name);
			}
		}
	}
}
