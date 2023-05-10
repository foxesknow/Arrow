using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Messaging;

public sealed class EndpointBuilder
{
    public static Uri MakeRequest(Uri baseEndpoint)
    {
        ArgumentNullException.ThrowIfNull(baseEndpoint);

        return AddToUri(baseEndpoint, ".Request");
    }

    public static Uri MakeResponse(Uri baseEndpoint)
    {
        ArgumentNullException.ThrowIfNull(baseEndpoint);

        return AddToUri(baseEndpoint, ".Response");
    }

    private static Uri AddToUri(Uri baseEndpoint, string extra)
    {
        var builder = new UriBuilder(baseEndpoint);
        builder.Path += extra;

        return builder.Uri;
    }
}
