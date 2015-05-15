using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server
{
	/// <summary>
	/// Features provides by the host to services that are running within it
	/// </summary>
	public interface IHost
	{
		/// <summary>
		/// Attempts to find a service implementing the specified interface.
		/// If multiple implementation are found the method will return false
		/// </summary>
		/// <typeparam name="T">The service interface to find</typeparam>
		/// <param name="service">On success a reference to a class implementing the service</param>
		/// <returns>True if the service is found, otherwise false</returns>
		bool TryDiscover<T>(out T service);
		
		
		/// <summary>
		/// Attepts to find a serive with the specified name that implements the specified service interface
		/// </summary>
		/// <typeparam name="T">The service interface to find</typeparam>
		/// <param name="serviceName">The name of the service to find</param>
		/// <param name="service">On success a reference to a class implementing the service</param>
		/// <returns>True if the service is found, otherwise false</returns>
		bool TryDiscover<T>(string serviceName, out T service);

		/// <summary>
		/// The name the service is exposed as
		/// </summary>
		public string Name{get;}

		/// <summary>
		/// The endpoint the host is listening on
		/// </summary>
		public Uri Endpoint{get;}

		/// <summary>
		/// Indicates that the calling service has encounted a fatal issue
		/// </summary>
		public void Fatal();
	}
}
