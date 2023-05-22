using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport;

/// <summary>
/// The data that is broadcast from the server
/// </summary>
public sealed class BroadcastData
{
    public BroadcastData(PublisherID publisherID, string nodeName, NodeDetails details)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        ArgumentNullException.ThrowIfNull(nodeName);
        ArgumentNullException.ThrowIfNull(details);

        PublisherID = publisherID;
        NodeName = nodeName;
        Details = details;
    }

    /// <summary>
    /// Who sent the data
    /// </summary>
    public PublisherID PublisherID{get;}

    /// <summary>
    /// The name of the node within details that holds the information for
    /// the broadcast manager who sent this information.
    /// </summary>
    public string NodeName{get;}

    /// <summary>
    /// The actual data
    /// </summary>
    public NodeDetails Details{get;}

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{NodeName} from {PublisherID}";
    }
}
