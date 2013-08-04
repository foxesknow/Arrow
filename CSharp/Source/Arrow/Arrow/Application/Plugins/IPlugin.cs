using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Defines a simple abstract plugin
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// Starts the plugin
		/// A plugin should gracefully handle being asked to start if it is already started
		/// </summary>
		void Start();
		
		/// <summary>
		/// Stops the plugin.
		/// A plugin should gracefully handle being asked to stop if it is already stopped
		/// </summary>
		void Stop();
		
		/// <summary>
		/// The name of the plugin
		/// </summary>
		string Name{get;}
	}
}
