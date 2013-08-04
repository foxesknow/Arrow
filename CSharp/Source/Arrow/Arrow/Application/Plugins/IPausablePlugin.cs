using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Application.Plugins
{
	/// <summary>
	/// Defines a simple abstract plugin that can be paused
	/// </summary>
	public interface IPausablePlugin : IPlugin
	{
		/// <summary>
		/// Pauses the service
		/// </summary>
		void Pause();
		
		/// <summary>
		/// Contines execution
		/// </summary>
		void Continue();
	}
}
