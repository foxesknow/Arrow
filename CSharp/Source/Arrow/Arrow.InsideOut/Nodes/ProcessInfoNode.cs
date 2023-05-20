using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Nodes;

/// <summary>
/// Returns information on the process
/// </summary>
public sealed class ProcessInfoNode : IInsideOutNode
{
    private readonly Value m_Pid;
    private readonly Value m_StartTime;

    public ProcessInfoNode()
    {
        m_Pid = Value.From(Environment.ProcessId);

        using(var self = System.Diagnostics.Process.GetCurrentProcess())
        {
            m_StartTime = Value.From(self.StartTime);
        }
    }

    public ValueTask<ExecuteResponse> Execute(ExecuteRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public ValueTask<NodeDetails> GetDetails(CancellationToken ct = default)
    {
        using(var self = System.Diagnostics.Process.GetCurrentProcess())
        {
            var details = new NodeDetails()
            {
                Values =
                {
                    {"PID", m_Pid},
                    {"StartTime", m_StartTime},
                    {"Gen-0 collections", Value.From(GC.CollectionCount(0))},
                    {"Gen-1 collections", Value.From(GC.CollectionCount(1))},
                    {"Gen-2 collections", Value.From(GC.CollectionCount(2))},
                    {"Managed Total Memory (K)", Value.From(ToKB(GC.GetTotalMemory(false)))},
                    {"Working Set (K)", Value.From(ToKB(self.WorkingSet64))},
                    {"Private Memory (K)", Value.From(ToKB(self.PrivateMemorySize64))},
                    {"User Procssor Time", Value.From(self.UserProcessorTime)},
                    {"Total Procssor Time", Value.From(self.TotalProcessorTime)},
                }
            };
        
            return new(details);
        }
    }

    private long ToKB(long bytes)
    {
        return bytes / 1024;
    }
}
