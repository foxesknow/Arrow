using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
	/// <summary>
	/// Setting from properties on the Environment object
	/// </summary>
	public class DotNetSettings : ISettings
	{
		/// <summary>
		/// An instance that may be shared
		/// </summary>		
		public static readonly ISettings Instance=new DotNetSettings();

		/// <summary>
		/// Retrieves an environment setting
		/// </summary>
		/// <param name="name">The name of the property on the Environment object</param>
		/// <returns>The value of the property</returns>
		public object? GetSetting(string name)
		{
			switch(name.ToLower())
			{
				case "cmdline":
				case "commandline":
					return Environment.CommandLine;

				case "osversion":
					return Environment.OSVersion;

				case "processorcount":
					return Environment.ProcessorCount;

				case "tickcount":
					return Environment.TickCount;

				case "userinteractive":
					return Environment.TickCount;

				case "username":
					return Environment.UserName;

				case "version":
					return Environment.Version;

				case "workingset":
					return Environment.WorkingSet;

				default:
					return null;
			}
		}
	}
}
