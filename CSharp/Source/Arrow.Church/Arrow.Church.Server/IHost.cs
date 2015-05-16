using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		/// <typeparam name="TService">The service interface to find</typeparam>
		/// <param name="service">On success a reference to a class implementing the service</param>
		/// <returns>True if the service is found, otherwise false</returns>
		bool TryDiscover<TService>(out TService service) where TService:class;
		
		
		/// <summary>
		/// Attempts to find a serive with the specified name that implements the specified service interface
		/// </summary>
		/// <typeparam name="TService">The service interface to find</typeparam>
		/// <param name="serviceName">The name of the service to find</param>
		/// <param name="service">On success a reference to a class implementing the service</param>
		/// <returns>True if the service is found, otherwise false</returns>
		bool TryDiscover<TService>(string serviceName, out TService service) where TService:class;

		/// <summary>
		/// Returns a list of all the services that implement the specified interface
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		IList<TService> DiscoverAll<TService>() where TService:class;

		/// <summary>
		/// The name the service is exposed as
		/// </summary>
		string ServiceName{get;}

		/// <summary>
		/// The endpoint the host is listening on
		/// </summary>
		Uri Endpoint{get;}

		/// <summary>
		/// Indicates that the calling service has encounted a fatal issue
		/// </summary>
		void Fatal();

		/// <summary>
		/// Signalled when the service should stop
		/// </summary>
		EventWaitHandle StopEvent{get;}

		/// <summary>
		/// Set when the service should stop
		/// </summary>
		CancellationToken StopCancellationToken{get;}
	}
}
