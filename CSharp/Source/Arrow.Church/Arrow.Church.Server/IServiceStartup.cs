using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common;

namespace Arrow.Church.Server
{
	/// <summary>
	/// Allows a service to be notified when it is being started
	/// </summary>
	public interface IServiceStartup
	{
		/// <summary>
		/// Called to allow the service to do any one time initialization
        /// <param name="serviceNameIdentifier">What the service is known as</param>
		/// </summary>
		void Start(ServiceNameIdentifier serviceNameIdentifier);

		/// <summary>
		/// Called when all service have been successfully initialized
		/// </summary>
		void AllStarted();
	}
}
