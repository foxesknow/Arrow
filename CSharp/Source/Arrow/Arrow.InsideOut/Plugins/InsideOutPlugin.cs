using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

using Arrow.Application.Plugins;
using Arrow.Execution;
using Arrow.Text;
using Arrow.Threading.Tasks;
using Arrow.Xml;
using Arrow.Xml.ObjectCreation;

namespace Arrow.InsideOut.Plugins;

/// <summary>
/// Manages all the registered InsideOut plugins.
/// 
/// Any installed transport layers will use this plugin to make calls into the InsideOut system.
/// 
/// By default all nodes that are registered will not support concurrent calls to GetDetails and Execute.
/// This will make it easier for applications to update state without having to deals with concurrency issues.
/// 
/// If a node wishes to allow concurrent calls to GetDetails and Execute then it should add the "AllowConcurrentCalls"
/// to its class declaration.
/// </summary>
public sealed partial class InsideOutPlugin : Plugin, IInsideOutPlugin, ICustomXmlInitialization, IPluginPostStart
{
    private readonly ConcurrentDictionary<string, IInsideOutNodeProxy> m_Nodes = new();

    private IReadOnlyList<IBroadcastPlugin> m_BroadcastPlugins = Array.Empty<IBroadcastPlugin>();

    /// <inheritdoc/>
    public override string Name
    {
        get{return nameof(InsideOutPlugin);}
    }

    /// <inheritdoc/>
    protected override ValueTask Start()
    {
        return default;
    }

    /// <summary>
    /// Unregisters any nodes that are currently registered
    /// and attempts to dispose them
    /// </summary>
    /// <returns></returns>
    protected override async ValueTask Stop()
    {
        foreach(var name in m_Nodes.Keys)
        {
            if(Unregister(name, out var node))
            {
                await Disposable.TryDisposeAsync(node).ContinueOnAnyContext();
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
    {
        if(request.TryPopLevel(out var name) == false) throw new InsideOutException("could not get a node name");

        if(m_Nodes.TryGetValue(name, out var proxy) == false) throw new InsideOutException($"could not find {name}");

        var result = await proxy.Execute(request, ct).ContinueOnAnyContext();

        if(m_BroadcastPlugins.Count != 0)
        {
            for(var i = 0; i < m_BroadcastPlugins.Count; i++)
            {
                var broadcaster = m_BroadcastPlugins[i];
                await broadcaster.SchedulePush().ContinueOnAnyContext();
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public async ValueTask<NodeDetails> GetDetails(CancellationToken ct)
    {
        var details = new NodeDetails();

        foreach(var (name, proxy) in m_Nodes)
        {
            ct.ThrowIfCancellationRequested();
                
            var innerDetails = await proxy.GetDetails(ct).ContinueOnAnyContext();
            details.Values.Add(name, innerDetails);
        }

        return details;
    }

    /// <inheritdoc/>
    public bool IsRegistered(string name)
    {
        return m_Nodes.ContainsKey(name);
    }

    /// <inheritdoc/>
    public bool Register(string name, IInsideOutNode node)
    {
        if(name is null) throw new ArgumentNullException(nameof(name));
        if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));
        if(node is null) throw new ArgumentNullException(nameof(node));

        var proxy = MakeProxy(node);
        if(m_Nodes.TryAdd(name, proxy))
        {
            return true;
        }

        proxy.Dispose();
        
        return false;
    }

    /// <inheritdoc/>
    public bool Unregister(string name, [NotNullWhen(true)] out IInsideOutNode? node)
    {
        if(m_Nodes.TryRemove(name, out var proxy))
        {
            node = proxy.WrappedItem;
            proxy.Dispose();
            
            return true;
        }

        node = null;
        return false;
    }

    private IInsideOutNodeProxy MakeProxy(IInsideOutNode node)
    {
        var attribute = node.GetType().GetCustomAttribute<AllowConcurrentCallsAttribute>();
        
        if(attribute is not null)
        {
            return new MultiThreadedProy(node);
        }
        else
        {
            return new SingleThreadedProxy(node);
        }
    }
   
    void ICustomXmlInitialization.InitializeObject(XmlNode rootNode, ICustomXmlCreation factory)
    {
        foreach(XmlNode? node in rootNode.SelectNodesOrEmpty("Node"))
        {
            if(node is null) continue;

            var name = node.Attributes!.GetValueOrDefault("name", "");
            if(string.IsNullOrWhiteSpace(name)) throw new InsideOutException("invalid node for node");

            var expanded = TokenExpander.ExpandText(name);
            var instance = factory.Create<IInsideOutNode>(node);

            if(Register(expanded, instance) == false)
            {
                throw new InsideOutException($"{expanded} is already registered");
            }
        }
    }

    ValueTask IPluginPostStart.AllPluginsStarted(IPluginDiscovery discovery)
    {
        m_BroadcastPlugins = discovery.FindAll<IBroadcastPlugin>();
        return default;
    }
}
