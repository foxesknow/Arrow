using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.ServiceRegistrar
{
    [Serializable]
    public sealed class RegisterRequest
    {
        public RegisterRequest(string serviceName, Uri endpoint)
        {
            if(serviceName==null) throw new ArgumentNullException("serviceName");
            if(string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException("invalid service name", "serviceName");
            if(endpoint==null) throw new ArgumentNullException("endpoint");

            this.ServiceName=serviceName;
            this.Endpoint=endpoint;
        }

        public string ServiceName{get;}

        public Uri Endpoint{get;private set;}
    }
}
