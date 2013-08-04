using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;
using Arrow.Application;

namespace Arrow.Settings
{
	/// <summary>
	/// Parses the command line and creates setting for any switches on the command line
	/// </summary>
	public class CommandLineSettings : ISettings
	{
		/// <summary>
		/// An instance of the class that may be shared
		/// </summary>
		public static readonly ISettings Instance=new CommandLineSettings();
	
		private Dictionary<string,object> m_Args=new Dictionary<string,object>(IgnoreCaseEqualityComparer.Instance);
	
		/// <summary>
		/// Initializes the instance
		/// </summary>
		public CommandLineSettings()
		{
			string[] args=Environment.GetCommandLineArgs();
			
			// The first one is the name of the executable, so ignore it
			for(int i=1; i<args.Length; i++)
			{
				string value=args[i];
				
				CommandLineSwitch commandSwitch;
				if(CommandLineSwitch.TryParse(value,out commandSwitch))
				{
					string commandValue=commandSwitch.Value ?? "";
					m_Args[commandSwitch.Name]=commandValue;
				}
			}
		}
	
		/// <summary>
		/// Retrieves a commandlune setting.
		/// </summary>
		/// <param name="name">The commandline setting name</param>
		/// <returns>A string instance, or null if the setting does not exist</returns>
		public object GetSetting(string name)
		{
			object value;
			m_Args.TryGetValue(name,out value);
			return value;
		}

	}
}
