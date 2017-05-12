using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common.Data.DotNet;

namespace Arrow.Church.Common.Services.ServiceRegistrar
{
    [ChurchService("ServiceRegistrar",typeof(SerializationMessageProtocol))]    
    public interface IServiceRegistrar
    {
        /// <summary>
        /// Registers a service
        /// </summary>
        /// <param name="request">The register request details</param>
        /// <returns>A response to the registratin</returns>
        Task<RegisterResponse> Register(RegisterRequest request);
        
        /// <summary>
        /// Unregisters a service
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task Unregister(UnregisterRequest request);

        /// <summary>
        /// Gets a service.
        /// If multiple services are registered with the same name one will be returned at random.
        /// If no such service exists then an exception is thrown.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GetServiceResponse> GetService(GetServiceRequest request);

        /// <summary>
        /// Gets all services that match the request
        /// If multiple services are registered then all their endpoints will be returned
        /// If no matching services exist then the response will contain an empty endpoint collection
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<GetServicesResponse> GetAllServices(GetServiceRequest request);
    }
}
