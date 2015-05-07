using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common.Net;
using Arrow.Church.Server.ServiceListeners;

namespace Arrow.Church.Server
{
	public class ServiceCallEventArgs : EventArgs
	{
		private readonly ServiceListener m_ServiceListener;
		private readonly CallDetails m_CallDetails;

		public ServiceCallEventArgs(ServiceListener serviceListener, CallDetails callDetails)
		{
			m_ServiceListener=serviceListener;
			m_CallDetails=callDetails;
		}

		public ServiceListener ServiceListener
		{
			get{return m_ServiceListener;}
		}

		public CallDetails CallDetails
		{
			get{return m_CallDetails;}
		}
	}
}
