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
        public GetServiceResponse(Uri endpoint)
        {
            if(endpoint==null) throw new ArgumentNullException("endpoint");

            this.Endpoint=endpoint;
        }

        /// <summary>
        /// The endpoint for a service
        /// </summary>
        public Uri Endpoint{get;private set;}
    }
}
