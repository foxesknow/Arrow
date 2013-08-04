using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Arrow.Settings
{
	/// <summary>
	/// Provides network related settings
	/// </summary>
	/// <remarks>
	/// The valid setting property names are:
	///		hostname
	///		ipaddress
	/// </remarks>
	public class NetworkSettings : ISettings
	{
		/// <summary>
		/// An instance that may be shared
		/// </summary>		
		public static readonly ISettings Instance=new NetworkSettings();

		/// <summary>
		/// Retrives a network setting.
		/// </summary>
		/// <param name="name">The network setting name</param>
		/// <returns>A string instance, or null if the setting does not exist</returns>
		public object GetSetting(string name)
		{
			switch(name.ToLower())
			{
				case "hostname":
					return Dns.GetHostEntry("").HostName;
					
				case "ipaddress":
					return Dns.GetHostEntry("").AddressList[0].ToString();
				
				default:
					return null;
			}
		}
	}
}
