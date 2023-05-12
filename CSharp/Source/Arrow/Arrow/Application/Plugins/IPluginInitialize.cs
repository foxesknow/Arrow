using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Allows a plugin to receive initialization information.
	/// This is typically used when a plugin will wish to make calls back into
	/// the controller that contains it, for example to look for related plugin
	/// </summary>
	public interface IPluginInitialize
	{
		/// <summary>
		/// Initializes the plugin just before Start() is called.
		/// Every call to Start() will be preceeded by a call to this method
		/// </summary>
		/// <param name="discovery">A interface to the discovery portion of the controller that owns the plugin</param>
		ValueTask Initialize(IPluginDiscovery discovery);
	}
}
