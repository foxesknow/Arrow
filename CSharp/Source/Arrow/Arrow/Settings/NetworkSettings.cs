using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Diagnostics.CodeAnalysis;

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
        public static readonly ISettings Instance = new NetworkSettings();

        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            switch(name.ToLower())
            {
                case "hostname":
                    value = Dns.GetHostEntry("").HostName;
                    return true;

                case "ipaddress":
                    value = Dns.GetHostEntry("").AddressList[0].ToString();
                    return true;

                default:
                    value = null;
                    return false;
            }
        }
    }
}
