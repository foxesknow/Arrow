using Arrow.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.Service
{
	/// <summary>
	/// Hosts a ServiceMain instance
	/// </summary>
	/// <typeparam name="TServiceMain"></typeparam>
	public sealed class ServiceMainHost<TServiceMain> : InteractiveServiceBase where TServiceMain:ServiceMain,new()
	{
		private TServiceMain m_ServiceMain;

		/// <summary>
		/// Starts the service
		/// </summary>
		/// <param name="args"></param>
		protected override void OnStart(string[] args)
		{
			m_ServiceMain=new TServiceMain();
			m_ServiceMain.OnStart(args);
		}

		/// <summary>
		/// Stops the service
		/// </summary>
		protected override void OnStop()
		{
			if(m_ServiceMain!=null)
			{
				m_ServiceMain.OnStop();

				IDisposable disposer=m_ServiceMain as IDisposable;
				if(disposer!=null)
				{
					MethodCall.AllowFail(disposer,d=>d.Dispose());
				}

				m_ServiceMain=null;
			}
		}
	}
}
