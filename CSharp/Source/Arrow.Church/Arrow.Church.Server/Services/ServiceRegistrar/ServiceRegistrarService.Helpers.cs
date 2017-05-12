using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Arrow.Church.Server.Services.ServiceRegistrar
{
    public sealed partial class ServiceRegistrarService
    {
        class Registration
        {
            public Uri Endpoint{get;set;}
            public OpaqueKey Key{get;set;}
        }

        class ServiceDetails
        {
            private int m_Next=0;

            public ServiceDetails()
            {
                this.Registration=new List<Registration>();
            }

            public Registration SelectByRoundRobin()
            {
                if(this.Registration.Count==0) return null;

                int next=(Interlocked.Increment(ref m_Next) & int.MaxValue);
                int index=next % this.Registration.Count;

                return this.Registration[index];

            }

            public List<Registration> Registration{get;private set;}
        }
    }
}
