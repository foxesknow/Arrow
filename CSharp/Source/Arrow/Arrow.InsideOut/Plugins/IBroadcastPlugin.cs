using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Plugins;

public interface IBroadcastPlugin
{
    /// <summary>
    /// Allows other plugins to schedule a broadcast push.
    /// Note that it is up to the actual implementation if it wants to do so.
    /// </summary>
    /// <returns></returns>
    public ValueTask SchedulePush();
}
