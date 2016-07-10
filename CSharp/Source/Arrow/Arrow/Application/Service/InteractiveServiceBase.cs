using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace Arrow.Application.Service
{
	/// <summary>
	/// Base class for services that wish to be run either as a service or as a Windows/Console application
	/// 
	/// Typically the classes InteractiveService and
	/// </summary>
	public class InteractiveServiceBase : ServiceBase
	{
		/// <summary>
		/// Starts the service by calling OnStart().
		/// This method exists to call down to OnStart() which is protected, so not available to the framework.
		/// </summary>
		/// <param name="args">Any arguments to the service</param>
		internal void DoStart(string[] args)
		{
			OnStart(args);
		}

		/// <summary>
		/// Stops the service by calling OnStop
		/// This method exists to call down to OnStop() which is protected, so not available to the framework.
		/// </summary>
		internal void DoStop()
		{
			OnStop();
		}
	}
}
