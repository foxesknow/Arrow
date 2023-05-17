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
        request.EnsureArgumentCount(1);
        var message = request.GetArgumentValue<string>(0);

        var result = new ExecuteResponse()
        {
            Success = true,
            Message = message.ToString()
        };

        return new(result);
    }

    public ValueTask<Details> GetDetails(CancellationToken ct)
    {
        var details = new Details()
        {
            Commands = 
            {
                Echo
            }
        };

        return new(details);
    }
}
