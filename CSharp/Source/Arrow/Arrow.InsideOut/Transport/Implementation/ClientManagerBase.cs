using Arrow.InsideOut.Transport.Http.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport.Implementation;

public abstract class ClientManagerBase : IDisposable
{
    private bool m_Disposed;

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
            NodeResponse.GetDetails => (Details)(transportResponse.Response),
            NodeResponse.Execute    => (ExecuteResponse)(transportResponse.Response),
            NodeResponse.Exception  => throw ((ExceptionResponse)(transportResponse.Response)).AsException(),
            _                       => throw new InsideOutException($"unsupported response type: {transportResponse.NodeResponse}")
        };
    }

    protected T ReturnExpected<T>(object? @object) where T : class
    {
        if(@object is T item) return item;

        throw new InsideOutException($"server did not return a {typeof(T).Name}");
    }

    protected bool IsDisposed
    {
        get{return m_Disposed;}
    }

    public virtual void Dispose()
    {
        m_Disposed = true;
    }

    protected void ThrowIfDisposed()
    {
        if(m_Disposed) throw new ObjectDisposedException(nameof(HttpClientManager));
    }
}
