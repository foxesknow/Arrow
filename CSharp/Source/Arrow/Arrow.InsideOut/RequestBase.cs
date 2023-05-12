using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// Base class for all requests sent into an InsideOut server
/// </summary>
[JsonPolymorphic]
[JsonDerivedType(typeof(ExecuteRequest), "Execute")]
public abstract class RequestBase
{
    private protected RequestBase()
    {
    }
}
