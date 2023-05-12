using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport;

public interface IClientManager
{
    /// <summary>
    /// Checks to see if publisher is registered with the client manager
    /// </summary>
    /// <param name="publisherID"></param>
    /// <returns></returns>
    public bool IsRegistered(PublisherID publisherID);

    /// <summary>
    /// Attempts to get the node for the given publisher
    /// </summary>
    /// <param name="publisherID"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool TryGetNode(PublisherID publisherID, [NotNullWhen(true)] out IInsideOutNode? node);

    /// <summary>
    /// Gets the node for the specified publisher
    /// </summary>
    /// <param name="publisherID"></param>
    /// <returns></returns>
    /// <exception cref="InsideOutException"></exception>
    public IInsideOutNode GetNode(PublisherID publisherID)
    {
        if(TryGetNode(publisherID, out var node)) return node;

        throw new InsideOutException($"not subscribed to {publisherID}");
    }
}
