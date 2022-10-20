using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Calendar;

namespace Arrow.Settings
{
	/// <summary>
	/// Retrieves either Now or UtcNow
	/// </summary>
	public class DateTimeSettings : ISettings
	{
		/// <summary>
		/// An instance that may be shared
		/// </summary>		
		public static readonly ISettings Instance=new DateTimeSettings();

		/// <summary>
		/// Retrieves a setting value
		/// </summary>
		/// <param name="name">The name of the setting to retrieve</param>
		/// <returns>The value of the setting, or null if it does not exist</returns>
		public object? GetSetting(string name)
		{
			switch(name.ToLower())
			{
				case "now":
					return Clock.Now;
					
				case "utcnow":
					return Clock.UtcNow;
					
				default:
					return null;
			}
		}
	}
}
