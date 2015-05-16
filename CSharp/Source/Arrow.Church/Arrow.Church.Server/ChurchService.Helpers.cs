using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Server
{
	public partial class ChurchService
	{
		class NotRunningHost : IHost
		{
			public static readonly IHost Instance=new NotRunningHost();

			bool IHost.TryDiscover<TService>(out TService service)
			{
				throw new InvalidOperationException("the host is not running");
			}

			bool IHost.TryDiscover<TService>(string serviceName, out TService service)
			{
				throw new InvalidOperationException("the host is not running");
			}

			IList<TService> IHost.DiscoverAll<TService>()
			{
				throw new InvalidOperationException("the host is not running");
			}

			string IHost.ServiceName
			{
				get{throw new InvalidOperationException("the host is not running");}
			}

			Uri IHost.Endpoint
			{
				get{throw new InvalidOperationException("the host is not running");}
			}

			void IHost.Fatal()
			{
				throw new InvalidOperationException("the host is not running");
			}

			System.Threading.EventWaitHandle IHost.StopEvent
			{
				get{throw new InvalidOperationException("the host is not running");}
			}

			System.Threading.CancellationToken IHost.StopCancellationToken
			{
				get{throw new InvalidOperationException("the host is not running");}
			}
		}
	}
}
