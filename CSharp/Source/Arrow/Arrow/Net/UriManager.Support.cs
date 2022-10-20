using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Net
{
    public partial class UriManager
    {
        class EndpointParameter
        {
            /// <summary>
            /// The name of the parameter
            /// </summary>
            public string? Name{get; set;}

            /// <summary>
            /// The value of the parameter
            /// </summary>
            public string? Value{get; set;}

            /// <summary>
            /// Ignore the parameter if the value is empty
            /// </summary>
            public bool IgnoreIfEmpty{get; set;} = true;
        }

        class EndpointDetails
        {
            /// <summary>
            /// A template to apply when processing endpoints
            /// </summary>
            public string? Template{get; set;}
            
            /// <summary>
            /// The name for the uri
            /// </summary>
            public string? Name{get; set;}
            
            /// <summary>
            /// The uri scheme
            /// </summary>
            public string? Scheme{get; set;}
            
            /// <summary>
            /// The host
            /// </summary>
            public string? Host{get; set;}
            
            /// <summary>
            /// The username
            /// </summary>
            public string? Username{get; set;}
            
            /// <summary>
            /// The password
            /// </summary>
            public string? Password{get; set;}
            
            /// <summary>
            /// The path to the resource
            /// </summary>
            public string? Path{get; set;}

            /// <summary>
            /// The port, if applicable
            /// </summary>
            public int? Port{get; set;}

            public Uri? Uri{get; set;}

            public List<EndpointParameter> Query = new();
        }
    }
}
