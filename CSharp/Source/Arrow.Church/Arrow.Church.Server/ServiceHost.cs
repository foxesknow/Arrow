using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Threading;

namespace Arrow.Church.Server
{
	public class ServiceHost
	{
		private readonly ServiceListener m_ServiceListener;
		private readonly ServiceContainer m_ServiceContainer=new ServiceContainer();

		private readonly ActionWorkQueue m_ServiceCallQueue=new ActionWorkQueue();

		public ServiceHost(ServiceListener serviceListener)
		{
			if(serviceListener!=null) throw new ArgumentNullException("serviceListener");

			m_ServiceListener=serviceListener;
			m_ServiceListener.ServiceCall+=HandleServiceCall;
		}

		public ServiceContainer ServiceContainer
		{
			get{return m_ServiceContainer;}
		}

		private void HandleServiceCall(object sender, ServiceCallEventArgs args)
		{
			m_ServiceCallQueue.Enqueue(()=>ProcessCall(args));
		}

		private void ProcessCall(ServiceCallEventArgs args)
		{
		}
	}
}
