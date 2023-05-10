using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// Base class for all responses sent back from an InsideOut server
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(Details), "Details")]
[JsonDerivedType(typeof(ExceptionResponse), "Exception")]
[JsonDerivedType(typeof(ExecuteResponse), "Execute")]
public abstract class ResponseBase : CompositeValue
{
    private protected ResponseBase()
    {
    }
}
