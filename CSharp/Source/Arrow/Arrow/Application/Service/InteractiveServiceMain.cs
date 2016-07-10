using Arrow.Execution;
using Arrow.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.Application.Service
{
	/// <summary>
	/// A service which doesn't spawn a thread to execute the Start/Stop methods.
	///
	/// Because of this it is the responsibility of Start() to not block,
	/// but return once it has started up the service infrastructure. This typically means
	/// that the Start() method will have spawned its own threads.
	/// 
	/// </summary>
	public abstract class InteractiveServiceMain : ServiceMain
	{
		private readonly ILog Log=LogManager.GetDefaultLog();
		private ManualResetEvent m_StopEvent;

		internal override void OnStart(string[] args)
		{
			m_StopEvent=new ManualResetEvent(false);
			
			Start(m_StopEvent,args);
		}

		internal override void OnStop()
		{
			try
			{
				m_StopEvent.Set();
				Stop();
			}
			catch(Exception e)
			{
				Log.Error("Error stopping interactive service",e);
			}

			m_StopEvent.Dispose();
			m_StopEvent=null;
		}
	}
}
