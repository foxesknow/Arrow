using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;
using Arrow.Messaging;
using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Messaging.Client;

/// <summary>
/// Allows you to receive broadcast messages over the messaging system
/// </summary>
public sealed partial class MessagingBroadcastManager : IDisposable
{
    private const string BroadcastName = Arrow.InsideOut.Transport.Messaging.Server.BroadcastPlugin.BroadcastName;

    private bool m_Disposed = ObjectDispose.SingleThreadedNotDisposed;

    private readonly object m_SyncRoot = new();
    private readonly Dictionary<PublisherID, BroadcastDataHandler> m_Subscriptions = new();
    private readonly Dictionary<Uri, MessageReceiver> m_Receivers = new();

    private readonly SequentialWorkQueue m_WorkQueue = new();

    /// <inheritdoc/>
    public void Dispose()
    {
        lock(m_SyncRoot)
        {
            if(ObjectDispose.TryDispose(ref m_Disposed))
            {
                foreach(var receiver in m_Receivers.Values)
                {
                    MethodCall.AllowFail(receiver, static receiver => receiver.Dispose());
                }

                m_Receivers.Clear();
                m_Subscriptions.Clear();
                m_WorkQueue.Dispose();
            }
        }
    }

    /// <summary>
    /// Subscribes to broadcast messages
    /// </summary>
    /// <param name="publisherID"></param>
    /// <param name="endpoint"></param>
    /// <param name="handler"></param>
    /// <exception cref="InsideOutException"></exception>
    public void Subscribe(PublisherID publisherID, Uri endpoint, BroadcastDataHandler handler)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        ArgumentNullException.ThrowIfNull(endpoint);
        ArgumentNullException.ThrowIfNull(handler);

        lock(m_SyncRoot)
        {
            ThrowIfDisposed();

            if(m_Subscriptions.ContainsKey(publisherID)) throw new InsideOutException($"already subscribed to {publisherID}");

            m_Subscriptions.Add(publisherID, handler);

            var broadcastEndpoint = EndpointBuilder.MakeBroadcast(endpoint);
            if(m_Receivers.ContainsKey(broadcastEndpoint) == false)
            {
                var receiver = MessagingSystem.CreateReceiver(broadcastEndpoint);
                m_Receivers.Add(broadcastEndpoint, receiver);
                receiver.MessageReceived += HandleMessageReceived;

                try
                {   
                    // This could fail for all manner of reasons (credentials, invalid url etc)
                    receiver.Connect(broadcastEndpoint);
                }
                catch
                {
                    receiver.MessageReceived -= HandleMessageReceived;
                    m_Receivers.Remove(broadcastEndpoint);
                    m_Subscriptions.Remove(publisherID);

                    // We should dispose the receiver just in case its got resources
                    // allocate to it even though it failed to connect
                    MethodCall.AllowFail(receiver, static receiver => receiver.Dispose());

                    throw;
                }
            }
        }
    }

    private void HandleMessageReceived(object? sender, MessageEventArgs args)
    {
        if(args.Message is IByteMessage byteMessage)
        {
            if(CanHandle(byteMessage, out var publisherID, out var handler))
            {
                var data = byteMessage.Data;
                m_WorkQueue.TryEnqueue(() => CallHandler(handler, publisherID, data));
            }
        }
    }

    private async ValueTask CallHandler(BroadcastDataHandler handler, PublisherID publisherID, byte[] buffer)
    {
        try
        {
            var details = InsideOutEncoder.Default.Decode<NodeDetails>(buffer, 0, buffer.Length);
            if(details is not null)
            {
                var broadcastData = new BroadcastData(publisherID, BroadcastName, details);
                await handler(broadcastData).ContinueOnAnyContext();
            }
        }
        catch(Exception)
        {
            // It's the job of the handler to deal with exceptions
        }
    }

    private bool CanHandle(IMessage message, [NotNullWhen(true)] out PublisherID? publisherID, [NotNullWhen(true)] out BroadcastDataHandler? handler)
    {
        if(message.GetProperty(nameof(PublisherID)) is string publisherAsString)
        {
            try
            {
                publisherID = PublisherID.Decode(publisherAsString);

                lock(m_SyncRoot)
                {
                    if(m_Subscriptions.TryGetValue(publisherID, out handler))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Fall through to not handling the message
            }
        }

        publisherID = null;
        handler = null;
        return false;
    }

    private void ThrowIfDisposed([CallerMemberName] string? reason = null)
    {
        ObjectDispose.ThrowIfDisposed(ref m_Disposed, nameof(MessagingBroadcastManager), reason);
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"Receive messages sent from {BroadcastName} plugin";
    }
}
