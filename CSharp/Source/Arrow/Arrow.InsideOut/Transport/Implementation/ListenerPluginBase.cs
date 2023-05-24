using Arrow.Application.Plugins;
using Arrow.InsideOut.Plugins;
using Arrow.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Implementation;

/// <summary>
/// Base class for server side listeners that receive requests
/// </summary>
public abstract class ListenerPluginBase : Plugin, IListener, IPluginInitialize
{
    private readonly ITransportProcessor m_Processor = new TransportProcessor();

    private IPluginDiscovery? m_PluginDiscovery;
    private PublisherID? m_PublisherID;

    private readonly IInsideOutNode? m_RootOverride;

    protected ListenerPluginBase()
    {
    }

    protected ListenerPluginBase(IInsideOutNode? rootOverride)
    {
        m_RootOverride = rootOverride;
    }

    /// <summary>
    /// The name of this instance
    /// </summary>
    public string? InstanceName{get; set;}

    public PublisherID? GetPublisherID()
    {
        return m_PublisherID;
    }

    /// <summary>
    /// The encoder to use
    /// </summary>
    protected InsideOutEncoder Encoder{get;} = new InsideOutEncoder();

    protected void Register(PublisherID publisherID)
    {
        m_PublisherID = publisherID;
    }

    ValueTask IPluginInitialize.Initialize(IPluginDiscovery discovery)
    {
        m_PluginDiscovery = discovery;
        return default;
    }

    /// <summary>
    /// Tries to find the root
    /// </summary>
    /// <returns></returns>
    protected IInsideOutNode? GetRoot()
    {
        if(m_RootOverride is not null) return m_RootOverride;

        if(m_PluginDiscovery is null) return null;

        return m_PluginDiscovery.Find<IInsideOutPlugin>();
    }

    /// <summary>
    /// Executes the incoming request
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    protected ValueTask<(bool Success, TransportResponse? Response)> Process(TransportRequest request, CancellationToken ct)
    {
        var publisherID = m_PublisherID;
        if(publisherID is null) return new((false, null));

        return DoProcess(publisherID, request, ct);
    }

    protected async ValueTask<(bool Success, TransportResponse? Response)> Process(byte[] buffer, int offset, int count, CancellationToken ct)
    {
        var publisherID = m_PublisherID;
        if(publisherID is null) return (false, null);

        var transportRequest = this.Encoder.Decode<TransportRequest>(buffer, offset, count);
        if(transportRequest is null) throw new InsideOutException("could not decode transport request");

        return await Process(transportRequest, ct).ContinueOnAnyContext();
    }

    private ValueTask<(bool Success, TransportResponse? Response)> DoProcess(PublisherID publisherID, TransportRequest request, CancellationToken ct)
    {
        // If it's not meant for us then bail out early
        if(publisherID.Equals(request.PublisherID) == false)
        {
            return new((false, null));
        }

        var root = GetRoot();
        return Process(root, m_Processor, request, ct);

        static async ValueTask<(bool Success, TransportResponse? Response)> Process(IInsideOutNode? root, ITransportProcessor processor, TransportRequest request, CancellationToken ct)
        {
            var response = await processor.Process(root, request, ct).ContinueOnAnyContext();
            return (true, response);
        }
    }
}
