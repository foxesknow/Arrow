using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Application.Service
{
	/// <summary>
	/// Implemented by any class that wishes to be exposed as a service.
	/// If the implementing class also implements IDisposable then the
	/// Dispose() method will be called during shutdown
	/// </summary>
	public abstract class ServiceMain
	{
		/// <summary>
		/// Called when the service should start
		/// </summary>
		/// <param name="stopEvent">An event that is set when the function should return</param>
		/// <param name="args">Any arguments to the service</param>
		protected abstract void Start(WaitHandle stopEvent, string[] args);

		/// <summary>
		/// Called when the service should stop
		/// </summary>
		protected abstract void Stop();

		internal abstract void OnStart(string[] args);

		internal abstract void OnStop();
	}
}
