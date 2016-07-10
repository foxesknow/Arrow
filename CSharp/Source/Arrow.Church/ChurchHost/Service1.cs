using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Arrow.Application.Service;
using Arrow.Church.Server;
using Arrow.Church.Server.HostBuilder;
using Arrow.Configuration;
using Arrow.Xml.ObjectCreation;

namespace ChurchHost
{
	public partial class ChurchHostService : InteractiveServiceBase
	{
		private ServiceHost m_Host;

		public ChurchHostService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			var hostConfigXml=AppConfig.GetSectionXml("App","Hosts/Main");
			var builder=XmlCreation.Create<ServiceHostBuilder>(hostConfigXml);

			m_Host=builder.Build();
			m_Host.Start();
		}

		protected override void OnStop()
		{
			if(m_Host!=null)
			{
				m_Host.Dispose();
				m_Host=null;
			}
		}
	}
}
