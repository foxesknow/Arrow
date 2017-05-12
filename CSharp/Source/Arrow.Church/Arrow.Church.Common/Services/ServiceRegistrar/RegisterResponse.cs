using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Arrow.Church.Common.Services.ServiceRegistrar
{
    [Serializable]
    public sealed class RegisterResponse
    {
        public RegisterResponse(OpaqueKey registrationKey)
        {
            if(registrationKey==null) throw new ArgumentNullException("registrationKey");

            this.RegistrationKey=registrationKey;
        }

        public OpaqueKey RegistrationKey{get;}
    }
}
