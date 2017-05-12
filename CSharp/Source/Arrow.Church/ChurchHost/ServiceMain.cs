using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application.Service;
using Arrow.Church.Server;
using Arrow.Church.Server.HostBuilder;
using Arrow.Configuration;
using Arrow.Logging;
using Arrow.Xml.ObjectCreation;
using Arrow.Church.Common.Services.ServiceRegistrar;
using Arrow.Church.Client;
using Arrow.Execution;
using Arrow.Church.Common;

namespace ChurchHost
{
	public class ServiceMain : InteractiveServiceMain
	{
        private static readonly ILog Log=LogManager.GetDefaultLog();

		private List<ServiceHost> m_RegisteredHosts=new List<ServiceHost>();
        private List<ServiceHost> m_UnregisteredHosts=new List<ServiceHost>();
        private IServiceRegistrar m_ServiceRegistrar;
        private List<OpaqueKey> m_RegistrationKeys=new List<OpaqueKey>();

		protected override void Start(System.Threading.WaitHandle stopEvent, string[] args)
		{
            m_UnregisteredHosts=CreateHosts("Host");
			foreach(var host in m_UnregisteredHosts)
            {
                host.Start();
            }

            m_RegisteredHosts=CreateHosts("RegisteredHost");
			foreach(var host in m_RegisteredHosts)
            {
                host.Start();
            }

            RegisterAllServices();
		}

		protected override void Stop()
		{
            if(m_ServiceRegistrar!=null && m_RegistrationKeys.Count!=0)
            {
                foreach(var key in m_RegistrationKeys)
                {
                    var request=new UnregisterRequest(key);
                    var task=m_ServiceRegistrar.Unregister(request);
                    task.Wait();
                }

                m_RegistrationKeys.Clear();
            }

            StopHostServices(m_UnregisteredHosts);
            StopHostServices(m_RegisteredHosts);
		}

        private void StopHostServices(IList<ServiceHost> hosts)
        {
            foreach(var host in hosts)
            {
                host.Dispose();
            }

            hosts.Clear();
        }

        private List<ServiceHost> CreateHosts(string section)
        {
            var hosts=new List<ServiceHost>();

            var builders=AppConfig.GetSectionObjects<ServiceHostBuilder>("App","Hosts",section);
            foreach(var builder in builders)
            {
                var host=builder.Build();
                hosts.Add(host);
            }

            return hosts;
        }

        /// <summary>
        /// Registers any hosts that are marked for registration with the service registrar
        /// </summary>
        private void RegisterAllServices()
        {
            var registrarDetails=AppConfig.GetSectionObject<ServiceNameIdentifier>("App","RegistrarDetails");

            var factory=ProxyManager.FactoryFor<IServiceRegistrar>();	
            
            string registrarServiceName=(registrarDetails.Name ?? WellKnownService.ServiceRegistrar);
			m_ServiceRegistrar=factory.Create(registrarDetails.Endpoint,registrarServiceName);

            foreach(var host in m_RegisteredHosts)
            {
                var endpoint=host.Endpoint;

                foreach(var serviceName in host.ServiceContainer.ServiceNames())
                {
                    var request=new RegisterRequest(serviceName,endpoint);
                    var task=m_ServiceRegistrar.Register(request);
                    m_RegistrationKeys.Add(task.Result.RegistrationKey);
                }
            }
        }
	}
}
