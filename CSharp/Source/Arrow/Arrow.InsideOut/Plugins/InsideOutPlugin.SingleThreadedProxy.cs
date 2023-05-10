using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Threading.Tasks;

namespace Arrow.InsideOut.Plugins;

public partial class InsideOutPlugin
{
    private sealed class SingleThreadedProxy : IInsideOutNodeProxy
    {
        private readonly AsyncLock m_Lock = new();
        private readonly IInsideOutNode m_Outer;

        public SingleThreadedProxy(IInsideOutNode outer)
        {
            m_Outer = outer;
        }

        public void Discard()
        {
            m_Lock.Dispose();
        }

        public void Dispose()
        {
            Discard();

            if(m_Outer is IDisposable d) d.Dispose();
        }

        public async ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using(await m_Lock)
            {
                return await m_Outer.Execute(request, ct).ContinueOnAnyContext();
            }
        }

        public async ValueTask<Details> GetDetails(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            using(await m_Lock)
            {
                return await m_Outer.GetDetails(ct).ContinueOnAnyContext();
            }
        }
    }
}
