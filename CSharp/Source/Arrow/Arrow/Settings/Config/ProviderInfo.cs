using System;
using System.Collections.Generic;
using System.Text;

namespace Arrow.Settings.Config
{
	/// <summary>
	/// Holds provider information thats been read from the app.config
	/// </summary>
	class ProviderInfo
	{
		/// <summary>
		/// The namespace to register the setting provider under
		/// </summary>
		public string? Namespace{get; set;}
		
		/// <summary>
		/// The settings provider to register
		/// </summary>
		public ISettings? Settings{get; set;}
	}
}
