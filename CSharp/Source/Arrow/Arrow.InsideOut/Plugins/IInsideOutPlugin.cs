using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Arrow.Application.Plugins;

namespace Arrow.InsideOut.Plugins;

public interface IInsideOutPlugin : IInsideOutNode
{
    /// <summary>
    /// Registers a node 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="node"></param>
    public bool Register(string name, IInsideOutNode node);

    /// <summary>
    /// Unregisters a node.
    /// The caller is responsible for disposing of the returned node
    /// </summary>
    /// <param name="name"></param>
    /// <param name="node">On success the node that was registered</param>
    /// <returns></returns>
    public bool Unregister(string name, [NotNullWhen(true)] out IInsideOutNode? node);

    /// <summary>
    /// Checks to see if a node is registered
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public bool IsRegistered(string name);
}
