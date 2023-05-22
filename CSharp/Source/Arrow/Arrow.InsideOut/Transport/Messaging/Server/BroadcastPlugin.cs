using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Application.Plugins;
using Arrow.Calendar;
using Arrow.Execution;
using Arrow.InsideOut.Plugins;
using Arrow.Logging;
using Arrow.Messaging;
using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Messaging.Server;

/// <summary>
/// Broadcast periodic updates over the messaging system
/// </summary>
public sealed class BroadcastPlugin : Plugin, IPluginPostStart, IPluginInitialize, IInsideOutNode, IAsyncDisposable, IBroadcastPlugin
{
    internal const string BroadcastName = "InsideOut.Messaging.BroadcastPlugin";

    private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), $"[{BroadcastName}]");

    private IReadOnlyList<PublisherID> m_PublisherIDs = Array.Empty<PublisherID>();

    private IInsideOutPlugin? m_InsideOutPlugin;

    private MessageSender? m_MessageSender;

    private SequentialWorkQueue m_WorkQueue = new();

    private readonly Reminders m_Reminders = new();

    private int m_Disposed = ObjectDispose.MultiThreadedNotDisposed;

    private long m_SequenceNumber = 0;
    private Value? m_CachedFrequency = null;
    private Value? m_CachedAllowSchedulePush = null;

    public override string Name
    {
        get{return BroadcastName;}
    }

    /// <summary>
    /// True to broadcast at startup, false to wait "Frequency" before doing so.
    /// Defaults to true
    /// </summary>
    public bool BroadcastOnStart{get; set;} = true;
    
    /// <summary>
    /// True to allow the SchedulePush method to actually schedule a push of broadcast data.
    /// Defaults to true
    /// </summary>
    public bool AllowSchedulePush{get; set;} = true;

    /// <summary>
    /// Where to broadcast to.
    /// The endpoint will be passed to the EndpointBuilder to get the final endpoint name
    /// </summary>
    public Uri? Endpoint{get; set;}

    /// <summary>
    /// How often to publish. Defaults to 15 seconds
    /// </summary>
    public TimeSpan Frequency{get; set;} = TimeSpan.FromSeconds(15);

    /// <inheritdoc/>
    protected override ValueTask Start()
    {
        if(m_InsideOutPlugin is not null)
        {
            // We'll register ourself with the inside out plugin so that
            // our state data is sent back to the client
            m_InsideOutPlugin.Register(BroadcastName, this);
        }
        
        return default;
    }

    /// <inheritdoc/>
    protected override ValueTask Stop()
    {
        return DisposeAsync();
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        if(ObjectDispose.TryDispose(ref m_Disposed))
        {
            m_InsideOutPlugin?.Unregister(BroadcastName, out var _);
            m_Reminders.Dispose();
            m_WorkQueue.Dispose();

            if(m_MessageSender is not null)
            {
                MethodCall.AllowFail(m_MessageSender, static sender => sender.Dispose());
                m_MessageSender = null;
            }
        }
        
        return default;
    }

    /// <summary>
    /// Determines if we're actually able to schedule a push
    /// </summary>
    /// <returns></returns>
    private bool CanSchedule()
    {
        return m_InsideOutPlugin is not null && 
               this.Endpoint is not null && 
               ObjectDispose.IsDisposed(ref m_Disposed) == false;
    }

    /// <inheritdoc/>
    ValueTask<ExecuteResponse> IInsideOutNode.Execute(ExecuteRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    ValueTask<NodeDetails> IInsideOutNode.GetDetails(CancellationToken ct)
    {
        var sequenceNumber = Interlocked.Increment(ref m_SequenceNumber);

        var frequency = (m_CachedFrequency ??= Value.From(this.Frequency));
        var allowSchedulePush = (m_CachedAllowSchedulePush ??= Value.From(this.AllowSchedulePush));

        var details = new NodeDetails()
        {
            Values = 
            {
                {BroadcastState.SequenceNumber, Value.From(sequenceNumber)},
                {BroadcastState.LastPublishedUtc, Value.From(Clock.UtcNow)},
                {BroadcastState.Frequency, frequency},
                {BroadcastState.AllowSchedulePush, allowSchedulePush},
            }
        };

        return new (details);
    }

    /// <summary>
    /// Works out who we're publishing for and starts broadcasting
    /// </summary>
    /// <param name="discovery"></param>
    /// <returns></returns>
    ValueTask IPluginPostStart.AllPluginsStarted(IPluginDiscovery discovery)
    {
        m_PublisherIDs = discovery.FindAll<IListener>()
                                  .Select(listener => listener.GetPublisherID()!)
                                  .Where(id => id is not null)
                                  .Distinct()
                                  .ToList();

        if(m_PublisherIDs.Count == 0)
        {
            Log.Warn("No publisher IDs were found");
        }

        if(CanSchedule())
        {
            if(this.BroadcastOnStart)
            {
                ScheduleBroadcastNow();
            }
            else
            {
                ScheduleBroadcast();
            }
        }

        return default;
    }

    /// <inheritdoc/>
    ValueTask IPluginInitialize.Initialize(IPluginDiscovery discovery)
    {
        m_InsideOutPlugin = discovery.Find<IInsideOutPlugin>();

        return default;
    }

    /// <inheritdoc/>
    ValueTask IBroadcastPlugin.SchedulePush()
    {
        if(this.AllowSchedulePush == false) return default;
        ScheduleOneOffPush();

        return default;
    }

    private bool TryGetSender([NotNullWhen(true)] out MessageSender? messageSender)
    {
        if(m_MessageSender is MessageSender existingSender)
        {
            messageSender = existingSender;
            return true;
        }

        if(this.Endpoint is null)
        {
            Log.Warn("no broadcast endpoint specified");
            messageSender = null;
            return false;
        }

        try
        {
            var broadcastEndpoint = EndpointBuilder.MakeBroadcast(this.Endpoint);
            m_MessageSender = MessagingSystem.CreateSender(broadcastEndpoint);
            m_MessageSender.Connect(broadcastEndpoint);
        }
        catch(Exception e)
        {
            Log.Error("failed to create sender", e);
        }

        messageSender = m_MessageSender;
        return messageSender is not null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reschedule"></param>
    /// <returns></returns>
    private async ValueTask BroadastDetails(bool reschedule)
    {
        try
        {
            var publisherIDs = m_PublisherIDs;
            if(publisherIDs.Count != 0 && TryGetSender(out var messageSender))
            {
                var memory = await GetSystemDetails().ContinueOnAnyContext();
                if(memory.IsEmpty == false)
                {
                    var buffer = memory.ToArray();

                    for(var i = 0; i < publisherIDs.Count; i++)
                    {
                        var message = messageSender.CreateByteMessage(buffer);
                        message.SetProperty(nameof(PublisherID), publisherIDs[i].Encode());
                        messageSender.Send(message);
                    }
                }
            }
        }
        catch(Exception e)
        {
            Log.Error("failed to broadcast details", e);
        }
        finally
        {
            if(reschedule) ScheduleBroadcast();
        }
    }

    /// <summary>
    /// Gets the NodeDetails for the entire application, as a buffer
    /// </summary>
    /// <returns></returns>
    private async ValueTask<Memory<byte>> GetSystemDetails()
    {
        var plugin = m_InsideOutPlugin;
        if(plugin is null) return null;

        var nodeDetails = await plugin.GetDetails().ContinueOnAnyContext();
        return InsideOutEncoder.Default.EncodeToMemory(nodeDetails);
    }

    /// <summary>
    /// Schedules a one-shot broadcast that won't be repeated
    /// </summary>
    private void ScheduleOneOffPush()
    {
        if(CanSchedule())
        {            
            MethodCall.AllowFail(() =>
            {
                var when = Clock.UtcNow;
                m_Reminders.Add(when, () => m_WorkQueue.TryEnqueue(() => BroadastDetails(false)));
            });
        }
    }

    /// <summary>
    /// Schedules a broadcast that will run now in the future, based on the configured frequency
    /// </summary>
    private void ScheduleBroadcast()
    {
        if(CanSchedule())
        {            
            MethodCall.AllowFail(() =>
            {
                var when = Clock.UtcNow + this.Frequency;
                m_Reminders.Add(when, () => m_WorkQueue.TryEnqueue(() => BroadastDetails(true)));
            });
        }
    }

    /// <summary>
    /// Schedules a broadcast that will run now, and then reschedule 
    /// itself based on the configured frequency
    /// </summary>
    private void ScheduleBroadcastNow()
    {
        if(CanSchedule())
        {            
            MethodCall.AllowFail(() =>
            {
                var when = Clock.UtcNow;
                m_Reminders.Add(when, () => m_WorkQueue.TryEnqueue(() => BroadastDetails(true)));
            });
        }
    }
}
