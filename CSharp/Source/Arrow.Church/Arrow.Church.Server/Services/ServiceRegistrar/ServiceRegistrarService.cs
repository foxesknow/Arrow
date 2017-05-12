using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Collections;
using Arrow.Execution;
using Arrow.Church.Common.Services.ServiceRegistrar;

namespace Arrow.Church.Server.Services.ServiceRegistrar
{
    public sealed partial class ServiceRegistrarService : ChurchService<IServiceRegistrar>, IServiceRegistrar
    {
        private readonly Dictionary<string,ServiceDetails> m_Services=new Dictionary<string,ServiceDetails>();
        private readonly Dictionary<OpaqueKey,string> m_Keys=new Dictionary<OpaqueKey,string>();
        private readonly object m_SyncRoot=new object();


        public Task<RegisterResponse> Register(RegisterRequest request)
        {
            if(request==null) throw new ArgumentNullException(nameof(request));

            var registration=new Registration()
            {
                Endpoint=request.Endpoint,
                Key=new OpaqueKey()
            };

            lock(m_SyncRoot)
            {
                if(m_Services.TryGetValue(request.ServiceName,out var serviceDetails))
                {
                    // There's potentially something already here. Check for duplicate endponts
                    var existing=serviceDetails.Registration.FirstOrDefault(r=>r.Endpoint==request.Endpoint);
                    if(existing!=null)
                    {
                        string message=string.Format("The service {0} is already registered on the endpoint {1}",request.ServiceName,request.Endpoint);
                        throw new RegistrarException(message);
                    }
                }
                else
                {
                    serviceDetails=new ServiceDetails();
                    m_Services.Add(request.ServiceName,serviceDetails);
                }

                serviceDetails.Registration.Add(registration);
                m_Keys.Add(registration.Key,request.ServiceName);
            }

            var response=new RegisterResponse(registration.Key);
            return Task.FromResult(response);
        }

        public Task Unregister(UnregisterRequest request)
        {
            if(request==null) throw new ArgumentNullException(nameof(request));

            lock(m_SyncRoot)
            {
                // First up, make sure it's a valid key
                if(m_Keys.TryGetValue(request.RegistrationKey,out var serviceName))
                {
                    // Now try and find the details for the specified key
                    if(m_Services.TryGetValue(serviceName,out var serviceDetails))
                    {
                        int index=serviceDetails.Registration.FindIndex(r=>r.Key==request.RegistrationKey);
                        if(index!=-1)
                        {
                            // We've found it
                            serviceDetails.Registration.RemoveAt(index);
                        }
                    }
                }

                // Tidy up the key cache
                m_Keys.Remove(request.RegistrationKey);
            }

            return Void();
        }

        public Task<GetServiceResponse> GetService(GetServiceRequest request)
        {
            if(request==null) throw new ArgumentNullException(nameof(request));

            Registration registration=null;

            lock(m_SyncRoot)
            {
                if(m_Services.TryGetValue(request.ServiceName,out var serviceDetails))
                {
                    registration=serviceDetails.SelectByRoundRobin();
                }
            }

            if(registration==null)
            {
                throw new RegistrarException("no such service registered: "+request.ServiceName);
            }

            var response=new GetServiceResponse(request.ServiceName,registration.Endpoint);
            return Task.FromResult(response);
        }

        public Task<GetServicesResponse> GetAllServices(GetServiceRequest request)
        {
            if(request==null) throw new ArgumentNullException(nameof(request));

            IList<Uri> endpoints=null;

            lock(m_SyncRoot)
            {
                if(m_Services.TryGetValue(request.ServiceName,out var serviceDetails))
                {
                    endpoints=serviceDetails.Registration.Select(r=>r.Endpoint).ToList();
                }
                else
                {
                    endpoints=new List<Uri>();
                }
            }

            var response=new GetServicesResponse(request.ServiceName,endpoints);
            return Task.FromResult(response);
        }
    }
}
