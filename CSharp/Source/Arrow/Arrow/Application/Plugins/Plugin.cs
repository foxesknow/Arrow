﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Defines a simple abstract plugin
	/// </summary>
	public abstract class Plugin
	{
		/// <summary>
		/// Starts the plugin
		/// A plugin should gracefully handle being asked to start if it is already started
		/// </summary>
		protected internal abstract ValueTask Start();
		
		/// <summary>
		/// Stops the plugin.
		/// A plugin should gracefully handle being asked to stop if it is already stopped
		/// </summary>
		protected internal abstract ValueTask Stop();
		
		/// <summary>
		/// The name of the plugin
		/// </summary>
		public abstract string Name{get;}
	}
}
