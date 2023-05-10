using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Plugins;

internal sealed class SingleThreadedNode : IInsideOutNode, IDisposable
{
    private readonly AsyncLock m_Lock = new();
    private readonly IInsideOutNode m_Outer;
    
    public SingleThreadedNode(IInsideOutNode outer)
    {
        m_Outer = outer;
    }

    /// <summary>
    /// Called by the plugin if the wrapper won't be used.
    /// We'll want to dispose of the lock, but not the outer instance
    /// </summary>
    internal void Discard()
    {
        m_Lock.Dispose();
    }

    public void Dispose()
    {
        if(m_Outer is IDisposable d)
        {
            d.Dispose();
        }
    }

    public async ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
    {
        using(await m_Lock)
        {
            return await m_Outer.Execute(request, ct);
        }
    }

    public async ValueTask<Details> GetDetails(CancellationToken ct)
    {
        using(await m_Lock)
        {
            return await m_Outer.GetDetails(ct);
        }
    }
}
