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
	/// A class that simplifies writing a service that runs in its own thread.
	/// 
	/// This class runs the Start/Stop methods in their own thread
	/// All calls to the service instance will be made on the same thread.
	/// </summary>
	public abstract class ThreadedServiceMain : ServiceMain
	{
		private readonly ILog Log=LogManager.GetDefaultLog();

		private Thread? m_ServiceThread;
		private ManualResetEvent? m_StopEvent;

		/// <summary>
		/// Starts the service
		/// </summary>
		/// <param name="args">Arguments to the service</param>
		internal override void OnStart(string[] args)
		{
			m_StopEvent=new ManualResetEvent(false);
			
			m_ServiceThread=new Thread(()=>RunService(args));
			m_ServiceThread.Name="ThreadedServiceMain";
			m_ServiceThread.Start();
		}

		/// <summary>
		/// Stops the service
		/// </summary>
		internal override void OnStop()
		{
			if(m_ServiceThread!=null)
			{
				m_StopEvent!.Set();
				m_ServiceThread.Join();

				m_StopEvent.Dispose();
				
				m_ServiceThread=null;
				m_StopEvent=null;
			}
		}

		private void RunService(string[] args)
		{
			try
			{
				Start(m_StopEvent!,args);

				// NOTE: If Start doesn't wait on the event then it will return and we'll drop down to
				// the finally block and potentially dispose the service. We only want that to happen
				// when the service is stopping, so by waiting here we can ensure that we only leave this
				// block when its time to stop
				m_StopEvent!.WaitOne();
			}
			catch(Exception e)
			{
				Log.Error("Error starting threaded service",e);
			}
			finally
			{
				try
				{
					Stop();
				}
				catch(Exception e)
				{
					Log.Error("Error stopping threaded service",e);
				}
			}
		}

	}
}
