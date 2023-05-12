using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Allows a plugin to be notified when all plugins in its host controller have been started.
	/// This is an optional interface for a plugin
	/// </summary>
	public interface IPluginPostStart
	{
		/// <summary>
		/// Called when all plugin have been started
		/// </summary>
		/// <param name="discovery">A interface to the discovery portion of the controller that owns the plugin</param>
		ValueTask AllPluginsStarted(IPluginDiscovery discovery);
	}
}
