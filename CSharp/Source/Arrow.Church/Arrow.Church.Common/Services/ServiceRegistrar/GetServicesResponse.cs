﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.ServiceRegistrar
{
    [Serializable]
    public sealed class GetServicesResponse
    {
        public GetServicesResponse(string serviceName, IEnumerable<Uri> endpoints)
        {
            if(serviceName==null) throw new ArgumentNullException(nameof(serviceName));
            if(endpoints==null) throw new ArgumentNullException(nameof(endpoints));

            foreach(var endpoint in endpoints)
            {
                if(endpoint==null) throw new ArgumentException("endpoints contains a null endpoint","endpoints");
            }

            this.ServiceName=serviceName;
            this.Endpoints=endpoints.ToList().AsReadOnly();
        }

        /// <summary>
        /// The name of the service
        /// </summary>
        public string ServiceName{get;}

        /// <summary>
        /// All the endpoints that match the request
        /// </summary>
        public IReadOnlyCollection<Uri> Endpoints{get;}
    }
}
