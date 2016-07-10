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
	public interface IServiceMain
	{
		/// <summary>
		/// Called when the service should run
		/// </summary>
		/// <param name="stopEvent">An event that is set when the function should return</param>
		/// <param name="args">Any arguments to the service</param>
		void Main(EventWaitHandle stopEvent, string[] args);
	}
}
