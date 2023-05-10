using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Execution;
using Arrow.InsideOut.Transport.Implementation;
using Arrow.Messaging;
using Arrow.Threading;

namespace Arrow.InsideOut.Transport.Messaging.Client;

public sealed partial class MessagingClientManager : ClientManagerBase, IClientManager
{
    private readonly string m_ApplicationID;

    private readonly InsideOutEncoder m_Encoder = new();

    private readonly ActionWorkQueue m_WorkQueue = new();

    private readonly object m_ActiveTasksSyncRoot = new();
    private readonly Dictionary<RequestID, TaskState> m_ActiveTasks = new();

    private readonly object m_MessagingSyncRoot = new();
    private readonly Dictionary<Uri, MessageSender> m_Requests = new();
    private readonly Dictionary<Uri, MessageReceiver> m_Responses = new();
    private readonly Dictionary<PublisherID, (Uri Request, Uri Response)> m_PublisherEndpoints = new();

    public MessagingClientManager()
    {
        m_ApplicationID = this.RequestIDFactory.ApplicationID.ToString();
    }

    public override void Dispose()
    {
        if(this.IsDisposed == false)
        {
            base.Dispose();

            lock(m_MessagingSyncRoot)
            {
                foreach(var sender in m_Requests.Values)
                {
                    MethodCall.AllowFail(sender, static sender => sender.Dispose());
                }

                m_Requests.Clear();

                foreach(var receiver in m_Responses.Values)
                {
                    MethodCall.AllowFail(receiver, static receiver => receiver.Dispose());
                }

                m_Responses.Clear();
            }

            CancelOutstandingTasks();
        }
    }

    public IInsideOutNode Register(PublisherID publisherID, Uri baseEndpoint)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        ArgumentNullException.ThrowIfNull(baseEndpoint);
        ThrowIfDisposed();

        var requestEndpoint = EndpointBuilder.MakeRequest(baseEndpoint);
        var responseEndpoint = EndpointBuilder.MakeResponse(baseEndpoint);

        lock(m_MessagingSyncRoot)
        {
            if(m_PublisherEndpoints.ContainsKey(publisherID)) throw new InsideOutException($"publisher already registered: {publisherID}");

            if(m_Requests.ContainsKey(requestEndpoint) == false)
            {
                var requests = MessagingSystem.CreateSender(requestEndpoint);
                requests.Connect(requestEndpoint);
                m_Requests.Add(requestEndpoint, requests);
            }

            if(m_Responses.ContainsKey(responseEndpoint) == false)
            {
                var responses = MessagingSystem.CreateReceiver(responseEndpoint);
                responses.MessageReceived += HandleMessage;
                responses.Connect(responseEndpoint);
                m_Responses.Add(responseEndpoint, responses);
            }

            m_PublisherEndpoints.Add(publisherID, (requestEndpoint, responseEndpoint));
        }

        return new Proxy(this, publisherID);
    }

    /// <inheritdoc/>
    public bool IsRegistered(PublisherID publisherID)
    {
        ArgumentNullException.ThrowIfNull(publisherID);

        lock(m_MessagingSyncRoot)
        {
            return m_PublisherEndpoints.ContainsKey(publisherID);
        }
    }

    /// <inheritdoc/>
    public bool TryGetNode(PublisherID publisherID, [NotNullWhen(true)] out IInsideOutNode? node)
    {
        ArgumentNullException.ThrowIfNull(publisherID);
        ThrowIfDisposed();

        lock(m_MessagingSyncRoot)
        {
            if(m_PublisherEndpoints.ContainsKey(publisherID))
            {
                node = new Proxy(this, publisherID);
                return true;
            }
        }

        node = null;
        return false;
    }

    private void HandleMessage(object? sender, MessageEventArgs args)
    {
        if(args.Message is IByteMessage message && CanProcess(message))
        {
            m_WorkQueue.TryEnqueue(() => Process(message.Data));
        }
    }

    private bool CanProcess(IMessage message)
    {
        return message.GetProperty(nameof(RequestID.ApplicationID)) is string applicationID && applicationID == m_ApplicationID;
    }

    private void Process(byte[] buffer)
    {
        try
        {
            var transportResponse = m_Encoder.Decode<TransportResponse>(buffer, 0, buffer.Length);
            if(transportResponse is null) throw new InsideOutException("response from server is null");

            if(transportResponse.RequestID.ApplicationID != this.RequestIDFactory.ApplicationID)
            {
                // It's not intended for us
                return;
            }

            switch(transportResponse.NodeResponse)
            {
                case NodeResponse.GetDetails:
                    GetDetailsResponse(transportResponse);
                    break;

                case NodeResponse.Execute:
                    ExecuteResponse(transportResponse);
                    break;

                case NodeResponse.Exception:
                    ExceptionResponse(transportResponse);
                    break;

                default:
                    var taskState = RemoveTaskState(transportResponse.RequestID);
                    if(taskState is not null)
                    {
                        taskState.TrySetException(new InsideOutException($"unsupported NodeResponse: {transportResponse.NodeResponse}"));
                    }
                    break;
            }
        }
        catch
        {
        }
    }

    private void GetDetailsResponse(TransportResponse transportResponse)
    {
        var taskState = RemoveTaskState<Details>(transportResponse.RequestID);
        if(taskState is null) return;

        if(transportResponse.Response is Details details)
        {
            taskState.Source.TrySetResult(details);
        }
        else
        {
            taskState.TrySetException(new InsideOutException("GetDetails did not return details"));
        }
    }

    private void ExecuteResponse(TransportResponse transportResponse)
    {
        var taskState = RemoveTaskState<ExecuteResponse>(transportResponse.RequestID);
        if(taskState is null) return;

        if(transportResponse.Response is ExecuteResponse executeResponse)
        {
            taskState.Source.TrySetResult(executeResponse);
        }
        else
        {
            taskState.TrySetException(new InsideOutException("Execute did not return details"));
        }
    }

    private void ExceptionResponse(TransportResponse transportResponse)
    {
        var taskState = RemoveTaskState(transportResponse.RequestID);
        if(taskState is null) return;

        if(transportResponse.Response is ExceptionResponse exceptionResponse)
        {
            var exception = exceptionResponse.AsException();
            taskState.TrySetException(exception);
        }
        else
        {
            taskState.TrySetException(new InsideOutException("Received an exception but no exception information was present"));
        }
    }

    private ValueTask<ExecuteResponse> Execute(PublisherID publisherID, ExecuteRequest executeRequest,  CancellationToken ct)
    {
        ThrowIfDisposed();

        var requestID = this.RequestIDFactory.Make();
        var taskState = MakeTaskState<ExecuteResponse>(requestID);

        var transportRequest = new TransportRequest(NodeFunction.Execute, publisherID, requestID)
        {
            Request = executeRequest
        };

        Send(publisherID, taskState, transportRequest, ct);

        return new(taskState.Source.Task);
    }

    private ValueTask<Details> GetDetails(PublisherID publisherID, CancellationToken ct)
    {
        ThrowIfDisposed();

        var requestID = this.RequestIDFactory.Make();
        var taskState = MakeTaskState<Details>(requestID);

        var transportRequest = new TransportRequest(NodeFunction.GetDetails, publisherID, requestID);
        Send(publisherID, taskState, transportRequest, ct);

        return new(taskState.Source.Task);
    }

    /// <summary>
    /// Sends the message to the appropriate endpoint
    /// </summary>
    /// <param name="publisherID"></param>
    /// <param name="taskState"></param>
    /// <param name="transportRequest"></param>
    /// <param name="ct"></param>
    private void Send(PublisherID publisherID, TaskState taskState, TransportRequest transportRequest, CancellationToken ct)
    {
        var sender = GetMessageSender(publisherID);
        if(sender is null)
        {
            RemoveTaskState(transportRequest.RequestID);
            taskState.TrySetException(new InsideOutException($"no sender for {publisherID}"));
            return;
        }

        try
        {
            var buffer = m_Encoder.EncodeToMemory(transportRequest);
            var message = sender.CreateByteMessage(buffer.ToArray());
            message.SetProperty(nameof(PublisherID), publisherID.Encode());

            sender.Send(message);

            // Register for cancellation so the user can cancel any outstanding calls
            if(ct != default)
            {
                ct.Register(state => CancelRequest((RequestID)state!), transportRequest.RequestID, false);
            }
        }
        catch(Exception e)
        {
            RemoveTaskState(transportRequest.RequestID);
            taskState.TrySetException(e);
        }
    }

    private MessageSender? GetMessageSender(PublisherID publisherID)
    {
        lock(m_MessagingSyncRoot)
        {
            if(m_PublisherEndpoints.TryGetValue(publisherID, out var endpoints))
            {
                if(m_Requests.TryGetValue(endpoints.Request, out var sender))
                {
                    return sender;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Cancels all outstanding request.
    /// Note that the processing on the server may still happen
    /// </summary>
    public void CancelOutstandingTasks()
    {
        lock(m_ActiveTasksSyncRoot)
        {
            foreach(var (requestID, state) in m_ActiveTasks)
            {
                state.TrySetException(new OperationCanceledException());
            }

            m_ActiveTasks.Clear();
        }
    }

    private TaskState<T> MakeTaskState<T>(RequestID requestID)
    {
        var taskState = new TaskState<T>();

        lock(m_ActiveTasks)
        {
            m_ActiveTasks.Add(requestID, taskState);
        }

        return taskState;
    }

    private TaskState? RemoveTaskState(RequestID requestID)
    {
        lock(m_ActiveTasks)
        {
            if(m_ActiveTasks.TryGetValue(requestID, out var taskState))
            {
                m_ActiveTasks.Remove(requestID);
                return taskState;
            }
        }

        return null;
    }

    private TaskState<T>? RemoveTaskState<T>(RequestID requestID)
    {
        lock(m_ActiveTasks)
        {
            if(m_ActiveTasks.TryGetValue(requestID, out var taskState) && taskState is TaskState<T> typedTaskState)
            {
                m_ActiveTasks.Remove(requestID);
                return typedTaskState;
            }
        }

        return null;
    }

    private void CancelRequest(RequestID requestID)
    {
        var taskState = RemoveTaskState(requestID);
        if(taskState is not null)
        {
            taskState.TrySetException(new OperationCanceledException());
        }
    }
}
