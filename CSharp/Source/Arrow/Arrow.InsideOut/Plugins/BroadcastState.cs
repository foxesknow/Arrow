using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Plugins;

/// <summary>
/// Defines the minimum information published by a broadcaster
/// </summary>
public static class BroadcastState
{
    /// <summary>
    /// An incrementing sequence number, which is different on each broadcast
    /// </summary>
    public const string SequenceNumber = nameof(SequenceNumber);
    
    /// <summary>
    /// How often the publisher is publishing data
    /// </summary>
    public const string Frequency = nameof(Frequency);
    
    /// <summary>
    /// The time the data was published, in UTC
    /// </summary>
    public const string LastPublishedUtc = nameof(LastPublishedUtc);

    /// <summary>
    /// Indicates if the broadcast plugin allows other plugins to schedule a push
    /// </summary>
    public const string AllowSchedulePush = nameof(AllowSchedulePush);
}
