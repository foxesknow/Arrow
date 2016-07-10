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
	/// This class takes a IServiceMain implementation and runs it in its own thread and
	/// manages the Start/Stop logic for the service
	/// </summary>
	/// <typeparam name="TServiceMain">The service to run</typeparam>
	public class ThreadedService<TServiceMain> : InteractiveServiceBase where TServiceMain:class,IServiceMain,new()
	{
		private readonly ILog Log=LogManager.GetDefaultLog();

		private Thread m_ServiceThread;
		private ManualResetEvent m_StopEvent;

		/// <summary>
		/// Starts the service
		/// </summary>
		/// <param name="args">Arguments to the service</param>
		protected override void OnStart(string[] args)
		{
			m_StopEvent=new ManualResetEvent(false);
			
			m_ServiceThread=new Thread(()=>RunService(args));
			m_ServiceThread.Name="ThreadedService";
			m_ServiceThread.Start();
		}

		/// <summary>
		/// Stops the service
		/// </summary>
		protected override void OnStop()
		{
			if(m_ServiceThread!=null)
			{
				m_StopEvent.Set();
				m_ServiceThread.Join();

				m_StopEvent.Dispose();
				
				m_ServiceThread=null;
				m_StopEvent=null;
			}
		}

		private void RunService(string[] args)
		{
			TServiceMain service=null;

			try
			{
				service=new TServiceMain();
				service.Main(m_StopEvent,args);
			}
			catch(Exception e)
			{
				Log.Error("Error running threaded service",e);
			}
			finally
			{
				// Make sure we dispose of the service, if required
				var disposable=service as IDisposable;
				if(disposable!=null)
				{
					MethodCall.AllowFail(disposable,(d)=>d.Dispose());
				}
			}
		}

	}
}
