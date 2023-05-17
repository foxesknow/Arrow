using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Execution;

namespace Arrow.InsideOut.Plugins;

public partial class InsideOutPlugin
{
    private interface IInsideOutNodeProxy : IInsideOutNode, IDisposable, IWrapper<IInsideOutNode>
    {
    }
}
