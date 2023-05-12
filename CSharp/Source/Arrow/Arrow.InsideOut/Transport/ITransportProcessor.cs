using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport;

/// <summary>
/// Takes an request from a transport layer and executes it
/// </summary>
public interface ITransportProcessor
{
    /// <summary>
    /// Processes a request
    /// </summary>
    /// <param name="root"></param>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public ValueTask<TransportResponse> Process(IInsideOutNode? root, TransportRequest request, CancellationToken ct);
}
