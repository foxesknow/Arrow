using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common;
using Arrow.Church.Common.Services.ServiceRegistrar;

namespace Arrow.Church.Client
{
    /// <summary>
    /// Uses a service registrar to provide access to services
    /// </summary>
    public class RegistrarProxyFactory
    {
        private IServiceRegistrar m_Service;

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="endpoint">The endpoint to the service registrar</param>
        public RegistrarProxyFactory(Uri endpoint)
        {
            if(endpoint==null) throw new ArgumentNullException(nameof(endpoint));

            var factory=ProxyManager.FactoryFor<IServiceRegistrar>();
            m_Service=factory.Create(endpoint);
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="registrarServiceName">The name of the service registrar</param>
        /// <param name="endpoint">The endpoint hosting the service</param>
        public RegistrarProxyFactory(string registrarServiceName, Uri endpoint)
        {
            if(registrarServiceName==null) throw new ArgumentNullException(nameof(registrarServiceName));
            if(string.IsNullOrWhiteSpace(registrarServiceName)) throw new ArgumentException(nameof(registrarServiceName));
            if(endpoint==null) throw new ArgumentNullException(nameof(endpoint));

            var factory=ProxyManager.FactoryFor<IServiceRegistrar>();
            m_Service=factory.Create(registrarServiceName, endpoint);
        }

        /// <summary>
        /// Gets a named service
        /// </summary>
        /// <typeparam name="TInterface">The interface the service implements</typeparam>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>A proxy interface to the service</returns>
        public TInterface GetService<TInterface>(string serviceName) where TInterface : class
        {
            var request=new GetServiceRequest(serviceName);
            var task=m_Service.GetService(request);
            var endpoint=task.Result.Endpoint;

            var factory=ProxyManager.FactoryFor<TInterface>();
            return factory.Create(serviceName,endpoint);
        }

        /// <summary>
        /// Asynchronously gets a named service
        /// </summary>
        /// <typeparam name="TInterface">The interface the service implements</typeparam>
        /// <param name="serviceName">The name of the service</param>
        /// <returns>A proxy interface to the service</returns>
        public async Task<TInterface> GetServiceAsync<TInterface>(string serviceName) where TInterface : class
        {
            var request=new GetServiceRequest(serviceName);
            var response=await m_Service.GetService(request).ConfigureAwait(false);

            var factory=ProxyManager.FactoryFor<TInterface>();
            return factory.Create(serviceName,response.Endpoint);
        }
    }
}
