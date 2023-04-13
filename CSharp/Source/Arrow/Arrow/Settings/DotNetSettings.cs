using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Arrow.Settings
{
    /// <summary>
    /// Setting from properties on the Environment object
    /// </summary>
    public sealed class DotNetSettings : ISettings
    {
        /// <summary>
        /// An instance that may be shared
        /// </summary>		
        public static readonly ISettings Instance = new DotNetSettings();

        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            switch(name.ToLower())
            {
                case "cmdline":
                case "commandline":
                    value = Environment.CommandLine;
                    return true;

                case "osversion":
                    value = Environment.OSVersion;
                    return true;

                case "processorcount":
                    value = Environment.ProcessorCount;
                    return true;

                case "tickcount":
                    value = Environment.TickCount;
                    return true;

                case "userinteractive":
                    value = Environment.UserInteractive;
                    return true;

                case "username":
                    value = Environment.UserName;
                    return true;

                case "version":
                    value = Environment.Version;
                    return true;

                case "workingset":
                    value = Environment.WorkingSet;
                    return true;

                default:
                    value = null;
                    return false;
            }
        }
    }
}
