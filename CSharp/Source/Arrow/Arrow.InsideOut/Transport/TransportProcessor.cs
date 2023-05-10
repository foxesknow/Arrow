using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Transport;

/// <summary>
/// A default implementation of a transport processor 
/// </summary>
public sealed class TransportProcessor : ITransportProcessor
{
    public ValueTask<TransportResponse> Process(IInsideOutNode? root, TransportRequest request, CancellationToken ct)
    {
        if(request is null) throw new ArgumentNullException(nameof(request));

        return Execute(root, request, ct);
        
        static async ValueTask<TransportResponse> Execute(IInsideOutNode? root, TransportRequest request, CancellationToken ct)
        {
            try
            {
                if(root is null) throw new InsideOutException("no root node");

                switch(request.NodeFunction)
                {
                    case NodeFunction.GetDetails:
                        if(request.Request is not null) throw new InsideOutException("GetDetails not not take any data");

                        var details = await root.GetDetails(ct).ContinueOnAnyContext();
                        return new(NodeResponse.GetDetails, request.RequestID, details);

                    case NodeFunction.Execute:
                        if(request.Request is ExecuteRequest executeRequest)
                        {
                            var result = await root.Execute(executeRequest, ct).ContinueOnAnyContext();
                            return new(NodeResponse.Execute, request.RequestID, result);
                        }

                        throw new InsideOutException("no ExecuteRequest data present");

                    default:
                        throw new InsideOutException($"unsupported function: {request.NodeFunction}");
                }
            }
            catch(Exception e)
            {
                var response = ExceptionResponse.From(e);
                return new(NodeResponse.Exception, request.RequestID, response);
            }
        }
    }
}
