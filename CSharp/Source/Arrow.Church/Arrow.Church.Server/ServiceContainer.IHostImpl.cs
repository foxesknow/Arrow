using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Church.Common;

namespace Arrow.Church.Server
{
	public partial class ServiceContainer
	{
		class Host : IHost
		{
			private readonly ServiceHost m_ServiceHost;
			private readonly ServiceData m_ServiceData;

			public Host(ServiceHost serviceHost, ServiceData serviceData)
			{
				m_ServiceHost=serviceHost;
				m_ServiceData=serviceData;
			}

			public bool TryDiscover<TService>(out TService service) where TService:class
			{
				var services=m_ServiceHost.ServiceContainer.DiscoverAll<TService>();
				
				if(services.Count==1)
				{
					service=services[0];
					return true;
				}
				else if(services.Count==0)
				{
					service=null;
					return false;
				}
				else
				{
					throw new ChurchException("IHost.TryDiscover - multiple serivce implementations found");
				}
			}

			public bool TryDiscover<TService>(string serviceName, out TService service) where TService:class
			{
				return m_ServiceHost.ServiceContainer.TryDiscover<TService>(serviceName,out service);
			}

			public IList<TService> DiscoverAll<TService>() where TService:class
			{
				var services=m_ServiceHost.ServiceContainer.DiscoverAll<TService>();
				return services;
			}

			public string ServiceName
			{
				get{return m_ServiceData.ServiceName;}
			}

			public Uri Endpoint
			{
				get{return m_ServiceHost.ServiceListener.Endpoint;}
			}

			public void Fatal()
			{
				throw new NotImplementedException();
			}

			public EventWaitHandle StopEvent
			{
				get{return m_ServiceHost.StopEvent;}
			}

			public CancellationToken StopCancellationToken
			{
				get{return m_ServiceHost.StopCancellationToken;}
			}
		}
	}
}
