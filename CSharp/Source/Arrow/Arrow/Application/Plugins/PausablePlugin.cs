using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Defines a simple abstract plugin that can be paused
	/// </summary>
	public abstract class PausablePlugin : Plugin
	{
		/// <summary>
		/// Pauses the service
		/// </summary>
		public abstract void Pause();
		
		/// <summary>
		/// Contines execution
		/// </summary>
		public abstract void Continue();
	}
}
