using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common
{
    /// <summary>
    /// Identifies a service by its name and endpoint
    /// </summary>
    [Serializable]
    public sealed class ServiceNameIdentifier
    {
        public string Name{get;set;}
        public Uri Endpoint{get;set;}
    }
}
