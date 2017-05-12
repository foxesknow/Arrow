using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.ServiceRegistrar
{
    [Serializable]
    public sealed class GetServicesResponse
    {
        public GetServicesResponse(IEnumerable<Uri> endpoints)
        {
            if(endpoints==null) throw new ArgumentNullException("endpoints");

            foreach(var endpoint in endpoints)
            {
                if(endpoint==null) throw new ArgumentException("endpoints contains a null endpoint","endpoints");
            }

            this.Endpoints=endpoints.ToList().AsReadOnly();
        }

        /// <summary>
        /// All the endpoints that match the request
        /// </summary>
        public IReadOnlyCollection<Uri> Endpoints{get;private set;}
    }
}
