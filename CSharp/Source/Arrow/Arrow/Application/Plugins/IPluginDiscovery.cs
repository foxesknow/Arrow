using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Provides plugin discovery.
	/// This is useful is you wish to expose a plugin controller in a "read only" manner.
	/// </summary>
	public interface IPluginDiscovery
	{
		/// <summary>
		/// Searches for a plugin that matches a predicate
		/// </summary>
		/// <param name="predicate">The predicate to apply to each plugin</param>
		/// <returns>The first plugin to match the predicate, or null if no plugin matches</returns>
		Plugin? Find(Func<Plugin, bool> predicate);
		
		/// <summary>
		/// Searches for the first service to implements the specified type
		/// </summary>
		/// <typeparam name="T">The type the required service must implement</typeparam>
		/// <returns>The first service to match, or null if no service matches</returns>
		T? Find<T>() where T : class;
		
		/// <summary>
		/// Searches for a plugin with a given name
		/// </summary>
		/// <param name="pluginName">The name of the plugin to find</param>
		/// <returns>The first plugin that matches the name, otherwise null</returns>
		Plugin? FindByName(string pluginName);
	}
}
