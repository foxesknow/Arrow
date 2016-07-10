using Arrow.Application.Service;
using Arrow.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrowHost
{
	public class ServiceMain : ThreadedServiceMain, IDisposable
	{
		private static ILog Log=LogManager.GetDefaultLog();

		protected override void Start(System.Threading.WaitHandle stopEvent, string[] args)
		{
			int id=1;

			do
			{
				Log.InfoFormat("ServiceMain - {0}",id);
				id++;
			}while(stopEvent.WaitOne(1000)==false);
		}

		protected override void Stop()
		{
			Log.Info("Stopping");
		}

		public void Dispose()
		{
			Log.Info("Disposing");
		}
	}
}
