using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Implementation;

/// <summary>
/// Generates RequestIDs
/// </summary>
public sealed class RequestIDFactory
{
    /// <summary>
    /// The current application id
    /// </summary>
    public Guid ApplicationID{get;} = Guid.NewGuid();

    /// <summary>
    /// Creates a new request id
    /// </summary>
    /// <returns></returns>
    public RequestID Make()
    {
        return new(this.ApplicationID, Guid.NewGuid());
    }

    /// <summary>
    /// Creates a request id for a specific application id
    /// </summary>
    /// <param name="applicationID"></param>
    /// <returns></returns>
    public RequestID Make(Guid applicationID)
    {
        return new(applicationID, Guid.NewGuid());
    }


    public override string ToString()
    {
        return this.ApplicationID.ToString();
    }
}
