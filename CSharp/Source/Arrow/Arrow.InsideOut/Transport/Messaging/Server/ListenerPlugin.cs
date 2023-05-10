using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Messaging;
using Arrow.InsideOut.Transport.Implementation;
using Arrow.Logging;
using Arrow.Execution;
using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Messaging.Server;

public sealed class ListenerPlugin : ListenerPluginBase, IDisposable, ISupportInitialize
{
    private const string ListenerName = "InsideOut.Messaging.ListenerPlugin";

    private static readonly ILog Log = new PrefixLog(LogManager.GetDefaultLog(), $"[{ListenerName}]");

    private Uri? m_RequestEndpoint;
    private Uri? m_ResponseEndpoint;

    private MessageReceiver? m_Requests;
    private MessageSender? m_Responses;

    private readonly SequentialWorkQueue m_WorkQueue = new();

    private readonly CancelSource m_CancelSource = new();

    public ListenerPlugin()
    {
    }

    internal ListenerPlugin(IInsideOutNode? rootOverride) : base(rootOverride)
    {
    }

    public void Dispose()
    {
        m_CancelSource.Dispose();
        m_WorkQueue.Dispose();
    }

    /// <summary>
    /// The base endpoint to listen on
    /// </summary>
    public Uri? Endpoint{get; init;}

    public override string Name => ListenerName;    

    protected override void Start()
    {
        if(this.InstanceName is null) throw new InsideOutException("no instance name specified");
        if(m_RequestEndpoint is null) throw new InsideOutException("could not resolve request endpoint");
        if(m_ResponseEndpoint is null) throw new InsideOutException("could not resolve response endpoint");

        var publisherID = new PublisherID(Environment.MachineName, this.InstanceName);
        Register(publisherID);

        if(m_Requests is null)
        {
            Log.Debug($"Receiving requests on {m_RequestEndpoint}");
            m_Requests = MessagingSystem.CreateReceiver(m_RequestEndpoint);
            m_Requests.MessageReceived += HandleMessage;
            m_Requests.Connect(m_RequestEndpoint);
        }

        if(m_Responses is null)
        {
            Log.Debug($"Sending responses on {m_ResponseEndpoint}");
            m_Responses = MessagingSystem.CreateSender(m_ResponseEndpoint);
            m_Responses.Connect(m_ResponseEndpoint);
        }
    }

    protected override void Stop()
    {
        m_CancelSource.Cancel();

        if(m_Requests is not null)
        {
            MethodCall.AllowFail(m_Requests, static d => d.Dispose());
            m_Requests = null;
        }

        if(m_Responses is not null)
        {
            MethodCall.AllowFail(m_Responses, static d => d.Dispose());
            m_Responses = null;
        }
    }

    private void HandleMessage(object? sender, MessageEventArgs args)
    {
        if(args.Message is IByteMessage message && CanProcess(message))
        {
            var buffer = message.Data;
            m_WorkQueue.TryEnqueue(() => HandleBuffer(buffer));
        }
    }

    private async ValueTask HandleBuffer(byte[] buffer)
    {
        try
        {
            var (success, response) = await Process(buffer, 0, buffer.Length, m_CancelSource.Token).ContinueOnAnyContext();
            if(success)
            {
                var memory = this.Encoder.EncodeToMemory(response);
                
                var message = m_Responses!.CreateByteMessage(memory.ToArray());
                message.SetProperty(nameof(RequestID.ApplicationID), response.RequestID.ApplicationID.ToString());
                m_Responses.Send(message);
            }
        }
        catch(Exception e)
        {
            Log.Error("failed to handle message", e);
        }
    }

    /// <summary>
    /// Tries to work out if we can process a message by looking for a PublisherID property
    /// in the message. This can help us avoid deserializing the message in order to make the check.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private bool CanProcess(IMessage message)
    {
        var publisherID = this.GetPublisherID();
        if(publisherID is null) return false;

        try
        {
            if(message.GetProperty(nameof(PublisherID)) is string id)
            {
                return PublisherID.Decode(id).Equals(publisherID);
            }
        }
        catch
        {
        }

        return true;
    }

    void ISupportInitialize.BeginInit()
    {
    }

    void ISupportInitialize.EndInit()
    {
        if(this.Endpoint is not null)
        {
            m_RequestEndpoint = EndpointBuilder.MakeRequest(this.Endpoint);
            m_ResponseEndpoint = EndpointBuilder.MakeResponse(this.Endpoint);
        }
    }

    internal void TestStart()
    {
        Start();
    }

    internal void TestStop()
    {
        Stop();
    }
}
