using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server
{
	/// <summary>
	/// Allows a service to be notified when it is shutting down
	/// </summary>
	public interface IServiceShutdown
	{
		/// <summary>
		/// Called to tell the service to shutdown
		/// </summary>
		void Shutdown();

		/// <summary>
		/// Called when all services have been shutdown
		/// </summary>
		void AllShutdown();
	}
}
