using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server
{
	/// <summary>
	/// Allows a service to be notified when it is being started
	/// </summary>
	public interface IServiceStartup
	{
		/// <summary>
		/// Called to allow the service to do any one time initialization
		/// </summary>
		void Start();

		/// <summary>
		/// Called when all service have been successfully initialized
		/// </summary>
		void AllStarted();
	}
}
