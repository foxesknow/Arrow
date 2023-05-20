using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut;

/// <summary>
/// Represents a node in an InsideOut server tree
/// </summary>
public interface IInsideOutNode
{
    /// <summary>
    /// Returns the details for the node
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public ValueTask<NodeDetails> GetDetails(CancellationToken ct = default);
    
    /// <summary>
    /// Executes a command against the node
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct = default);
}
