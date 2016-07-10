using Arrow.Application.Service;
using Arrow.Church.Server;
using Arrow.Church.Server.HostBuilder;
using Arrow.Configuration;
using Arrow.Xml.ObjectCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChurchHost
{
	public class ServiceMain : InteractiveServiceMain
	{
		private ServiceHost m_Host;

		protected override void Start(System.Threading.WaitHandle stopEvent, string[] args)
		{
			var hostConfigXml=AppConfig.GetSectionXml("App","Hosts/Main");
			var builder=XmlCreation.Create<ServiceHostBuilder>(hostConfigXml);

			m_Host=builder.Build();
			m_Host.Start();
		}

		protected override void Stop()
		{
			if(m_Host!=null)
			{
				m_Host.Dispose();
				m_Host=null;
			}
		}
	}
}
