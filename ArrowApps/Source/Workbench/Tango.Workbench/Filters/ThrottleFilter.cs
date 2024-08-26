﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Tango.Workbench.Filters
{
    /// <summary>
    /// Throttle the pipeline, ensuring that only "Quantity" items are returned every "Interval"
    /// </summary>
    [Filter("Throttle")]
    public sealed class ThrottleFilter : Filter
    {
        public override IAsyncEnumerable<object> Run(IAsyncEnumerable<object> items)
        {
            if(this.Quantity <= 0 || this.Interval <= TimeSpan.Zero) return items;

            return Throttle(this.Quantity, this.Interval.TotalMilliseconds, items);
        }

        private async IAsyncEnumerable<object> Throttle(int quantity, double intervalMs, IAsyncEnumerable<object> items)
        {
            var pushed = 0;
            var start = ExecutionTimer.Now;

            await foreach(var item in items)
            {
                yield return item;
                pushed++;

                if(pushed == quantity)
                {
                    var stop = ExecutionTimer.Now;
                    var elapsedMs = ExecutionTimer.ElapsedMilliseconds(start, stop);

                    if(elapsedMs < intervalMs)
                    {
                        var delayMs = intervalMs - elapsedMs;
                        await Task.Delay(TimeSpan.FromMilliseconds(delayMs), this.Context.CancellationToken);
                    }

                    pushed = 0;
                    start = ExecutionTimer.Now;
                }
            }
        }

        /// <summary>
        /// How many items to return during the interval
        /// </summary>
        public int Quantity{get; set;}

        /// <summary>
        /// The window of time over which the quantity can be returned.
        /// Once that quantity has been returned the throttle will pause for the remaining time.
        /// </summary>
        public TimeSpan Interval{get; set;}
    }
}