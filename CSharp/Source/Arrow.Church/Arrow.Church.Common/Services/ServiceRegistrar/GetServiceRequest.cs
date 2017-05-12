using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.ServiceRegistrar
{
    [Serializable]
    public sealed class GetServiceRequest
    {
        public GetServiceRequest(string serviceName)
        {
            if(serviceName==null) throw new ArgumentNullException(nameof(serviceName));
            if(string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException(nameof(serviceName));

            this.ServiceName=serviceName;
        }
        
        /// <summary>
        /// The name of the service to get
        /// </summary>
        public string ServiceName{get;}
    }
}
