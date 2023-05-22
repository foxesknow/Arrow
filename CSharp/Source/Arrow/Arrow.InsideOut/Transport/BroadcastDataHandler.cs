using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Transport
{
    public delegate ValueTask BroadcastDataHandler(BroadcastData broadcastData);
}
