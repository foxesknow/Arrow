using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;

namespace Arrow.Church.Server.HostBuilder
{
	public class ServiceHostBuilder
	{
		public ServiceHostBuilder()
		{
			this.Services=new List<ServiceDetails>();
		}

		public Uri Endpoint{get;set;}
		public List<ServiceDetails> Services{get;private set;}

		public ServiceHost Build()
		{
			if(this.Endpoint==null) throw new ChurchException("ServiceHostBuilder.Build - no endpoint specified");

			var host=new ServiceHost(this.Endpoint);

			foreach(var details in this.Services)
			{
				var service=details.Service;
				if(service==null) throw new ChurchException("ServiceHostBuilder.Build - service is null");

				var name=details.Name;
				if(name==null)
				{
					host.ServiceContainer.Add(service);
				}
				else
				{
					host.ServiceContainer.Add(name,service);
				}
			}

			return host;
		}
	}
}
