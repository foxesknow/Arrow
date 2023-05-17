using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Messaging;

/// <summary>
/// Builds endpoints for the messaging transport
/// </summary>
public sealed class EndpointBuilder
{
    /// <summary>
    /// Given a base endpoint creates the request endpoint
    /// </summary>
    /// <param name="baseEndpoint"></param>
    /// <returns></returns>
    public static Uri MakeRequest(Uri baseEndpoint)
    {
        ArgumentNullException.ThrowIfNull(baseEndpoint);

        return AddToUri(baseEndpoint, ".Request");
    }

    /// <summary>
    /// Given a base endpoint creates the response endpoint
    /// </summary>
    /// <param name="baseEndpoint"></param>
    /// <returns></returns>
    public static Uri MakeResponse(Uri baseEndpoint)
    {
        ArgumentNullException.ThrowIfNull(baseEndpoint);

        return AddToUri(baseEndpoint, ".Response");
    }

    /// <summary>
    /// Given a base endpoint creates the broadcast endpoint
    /// </summary>
    /// <param name="baseEndpoint"></param>
    /// <returns></returns>
    public static Uri MakeBroadcast(Uri baseEndpoint)
    {
        ArgumentNullException.ThrowIfNull(baseEndpoint);

        return AddToUri(baseEndpoint, ".Broadcast");
    }

    private static Uri AddToUri(Uri baseEndpoint, string extra)
    {
        var builder = new UriBuilder(baseEndpoint);
        builder.Path += extra;

        return builder.Uri;
    }
}
