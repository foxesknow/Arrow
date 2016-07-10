using Arrow.Application.Service;
using Arrow.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrowHost
{
	public class ServiceMain : IServiceMain
	{
		public void Main(System.Threading.EventWaitHandle stopEvent, string[] args)
		{
			ILog Log=LogManager.GetDefaultLog();

			int id=1;

			do
			{
				Log.InfoFormat("ServiceMain - {0}",id);
				id++;
			}while(stopEvent.WaitOne(1000)==false);
		}
	}
}
