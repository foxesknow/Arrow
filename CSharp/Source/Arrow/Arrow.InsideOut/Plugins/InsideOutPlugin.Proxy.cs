using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.InsideOut.Plugins;

public partial class InsideOutPlugin
{
    private interface IInsideOutNodeProxy : IInsideOutNode, IDisposable
    {
        /// <summary>
        /// Discard disposes of any proxy resources, but not of the underlying node
        /// </summary>
        public void Discard();
    }
}
