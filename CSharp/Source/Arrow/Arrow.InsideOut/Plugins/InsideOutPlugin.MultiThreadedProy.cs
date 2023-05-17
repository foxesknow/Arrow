using Arrow.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Plugins;

public partial class InsideOutPlugin
{
    private sealed class MultiThreadedProy : IInsideOutNodeProxy
    {
        private readonly IInsideOutNode m_Outer;

        public MultiThreadedProy(IInsideOutNode outer)
        {
            m_Outer = outer;
        }

        public IInsideOutNode WrappedItem
        {
            get{return m_Outer;}
        }

        public void Dispose()
        {
        }

        public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return m_Outer.Execute(request, ct);
        }

        public ValueTask<Details> GetDetails(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return m_Outer.GetDetails(ct);
        }
    }
}
