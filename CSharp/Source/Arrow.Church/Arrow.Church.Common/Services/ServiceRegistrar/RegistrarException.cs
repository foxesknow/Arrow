using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Common.Services.ServiceRegistrar
{
    [Serializable]
    public class RegistrarException : Exception
    {
        public RegistrarException() { }
        public RegistrarException(string message) : base(message) { }
        public RegistrarException(string message, Exception inner) : base(message, inner) { }
        protected RegistrarException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
