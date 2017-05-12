using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.ServiceRegistrar
{
    [Serializable]
    public sealed class GetServiceResponse
    {
        public GetServiceResponse(string serviceName, Uri endpoint)
        {
            if(serviceName==null) throw new ArgumentNullException(nameof(serviceName));
            if(string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException(nameof(serviceName));
            if(endpoint==null) throw new ArgumentNullException(nameof(endpoint));

            this.ServiceName=serviceName;
            this.Endpoint=endpoint;
        }

        /// <summary>
        /// The name of the service
        /// </summary>
        public string ServiceName{get;}

        /// <summary>
        /// The endpoint for a service
        /// </summary>
        public Uri Endpoint{get;}
    }
}
