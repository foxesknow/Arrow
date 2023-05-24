using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Arrow.InsideOut.Transport.Implementation;

public abstract class ClientManagerBase : IDisposable
{
    private int m_Disposed = ObjectDispose.MultiThreadedNotDisposed;

    protected RequestIDFactory RequestIDFactory{get;} = new();

    /// <summary>
    /// Extracts the response from the transport layer.
    /// If the response represents an exception then that exception is thrown.
    /// </summary>
    /// <param name="transportResponse"></param>
    /// <returns></returns>
    /// <exception cref="InsideOutException"></exception>
    protected object? ExtractResponseData(TransportResponse? transportResponse)
    {
        if(transportResponse is null) throw new InsideOutException("no transport response received");

        return transportResponse.NodeResponse switch
        {
            NodeResponse.GetDetails => (NodeDetails)(transportResponse.Response),
            NodeResponse.Execute    => (ExecuteResponse)(transportResponse.Response),
            NodeResponse.Exception  => throw ((ExceptionResponse)(transportResponse.Response)).AsException(),
            _                       => throw new InsideOutException($"unsupported response type: {transportResponse.NodeResponse}")
        };
    }

    protected InsideOutEncoder Encoder{get;} = new();

    protected T ReturnExpected<T>(object? @object) where T : class
    {
        if(@object is T item) return item;

        throw new InsideOutException($"server did not return a {typeof(T).Name}");
    }

    /// <summary>
    /// Called if the manager should dispose of its resources.
    /// It is guaranteed that this method will only be called once
    /// </summary>
    protected virtual void DisposeManager()
    {
    }

    /// <summary>
    /// Disposes of the client manager
    /// </summary>
    public void Dispose()
    {
        if(ObjectDispose.TryDispose(ref m_Disposed))
        {
            DisposeManager();
        }
    }

    /// <summary>
    /// Throws an exception if the manager has been disposed
    /// </summary>
    /// <param name="reason"></param>
    /// <exception cref="ObjectDisposedException"></exception>
    protected void ThrowIfDisposed([CallerMemberName] string? reason = null)
    {
        if(ObjectDispose.IsDisposed(ref m_Disposed)) 
        {
            var typeName = this.GetType().Name;
            throw new ObjectDisposedException(typeName, reason);
        }
    }
}
