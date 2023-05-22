using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Plugins;

public static class BroadcastState
{
    public const string SequenceNumber = nameof(SequenceNumber);
    public const string Frequency = nameof(Frequency);
    public const string LastPublished = nameof(LastPublished);
    public const string AllowSchedulePush = nameof(AllowSchedulePush);
}
