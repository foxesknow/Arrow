using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Nodes;

[AllowConcurrentCalls]
public sealed class EchoNode : IInsideOutNode
{
    private static readonly Command Echo = new("Echo")
    {
        Description = "Sends back the message you send it",
        Parameters =
        {
            new StringParameter("message"){Description = "The message to send back"}
        }
    };

    public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
    {
        var command = request.PopLeafLevel();
        
        var message = command switch
        {
            "Echo" => request.Let((string incoming) => incoming),
            _      => throw new InsideOutException($"unsupported operation: {command}")
        };

        var result = new ExecuteResponse()
        {
            Success = true,
            Message = message.ToString()
        };

        return new(result);
    }

    public ValueTask<NodeDetails> GetDetails(CancellationToken ct)
    {
        var details = new NodeDetails()
        {
            Commands = 
            {
                Echo
            }
        };

        return new(details);
    }
}
