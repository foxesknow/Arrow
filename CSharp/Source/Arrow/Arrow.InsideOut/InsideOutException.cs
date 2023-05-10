using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

[Serializable]
public class InsideOutException : Exception
{
    public InsideOutException() { }
    public InsideOutException(string message) : base(message) { }
    public InsideOutException(string message, Exception inner) : base(message, inner) { }
    protected InsideOutException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
