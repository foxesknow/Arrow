using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace Arrow.Application.Services
{
	/// <summary>
	/// Base class for services that wish to be run either as a service
	/// or as a Windows/Console application
	/// </summary>
	public class InteractiveServiceBase : ServiceBase
	{
		/// <summary>
		/// Starts the service by calling OnStart()
		/// </summary>
		/// <param name="args">Any arguments to the service</param>
		protected internal virtual void DoStart(string[] args)
		{
			OnStart(args);
		}

		/// <summary>
		/// Stops the service by calling OnStop
		/// </summary>
		protected internal virtual void DoStop()
		{
			OnStop();
		}
	}
}
