using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
	/// <summary>
	/// Retrieves and generates guid
	/// </summary>
	/// <remarks>
	/// The valid setting names are:
	/// <list type="bullet">
	///		<item>app - a guid that is constant whilst the app is running</item>
	///		<item>new - generates a new guid</item>
	///	</list>
	/// </remarks>
	public class GuidSettings : ISettings
	{
		private static Guid s_AppGuid=Guid.NewGuid();
		
		/// <summary>
		/// An instance of the class that may be shared
		/// </summary>
		public static readonly ISettings Instance=new GuidSettings();

		/// <summary>
		/// Retrieves a guid setting
		/// </summary>
		/// <param name="name">The guid setting to retrieve</param>
		/// <returns>A guid instance, or null if the setting does not exist</returns>
		public object? GetSetting(string name)
		{
			switch(name.ToLower())
			{
				case "app":
					return s_AppGuid;
					
				case "new":
					return Guid.NewGuid();
					
				default:
					return null;
			}
		}
	}
}
