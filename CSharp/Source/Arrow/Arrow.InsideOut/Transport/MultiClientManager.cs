using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Transport;

/// <summary>
/// Manages multiple client managers as a single client manager
/// </summary>
public sealed class MultiClientManager : IClientManager, IDisposable
{
    private readonly IReadOnlyList<IClientManager> m_Clients;

    /// <summary>
    /// Initializes the instance
    /// </summary>
    /// <param name="clientManagers"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public MultiClientManager(IEnumerable<IClientManager> clientManagers)
    {
        if(clientManagers is null) throw new ArgumentNullException(nameof(clientManagers));

        m_Clients = clientManagers.ToList();
        if(m_Clients.Any(client => client is null)) throw new ArgumentException("null client in sequence", nameof(clientManagers));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        foreach(var client in m_Clients)
        {
            if(client is IDisposable d) d.Dispose();
        }
    }

    /// <inheritdoc/>
    public bool IsRegistered(PublisherID publisherID)
    {
        for(var i = 0; i < m_Clients.Count; i++)
        {
            if(m_Clients[i].IsRegistered(publisherID)) return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public bool TryGetNode(PublisherID publisherID, [NotNullWhen(true)] out IInsideOutNode? node)
    {
        for(var i = 0; i < m_Clients.Count; i++)
        {
            if(m_Clients[i].TryGetNode(publisherID, out node)) return true;
        }

        node = null;
        return false;
    }
}
