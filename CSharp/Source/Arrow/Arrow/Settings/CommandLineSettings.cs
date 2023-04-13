using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;
using Arrow.Application;
using System.Diagnostics.CodeAnalysis;

namespace Arrow.Settings
{
    /// <summary>
    /// Parses the command line and creates setting for any switches on the command line
    /// </summary>
    public sealed class CommandLineSettings : ISettings
    {
        /// <summary>
        /// An instance of the class that may be shared
        /// </summary>
        public static readonly ISettings Instance = new CommandLineSettings();

        private readonly Dictionary<string, object> m_Args = new Dictionary<string, object>(IgnoreCaseEqualityComparer.Instance);

        /// <summary>
        /// Initializes the instance
        /// </summary>
        public CommandLineSettings()
        {
            string[] args = Environment.GetCommandLineArgs();

            // The first one is the name of the executable, so ignore it
            for(int i = 1; i < args.Length; i++)
            {
                var value = args[i];

                if(CommandLineSwitch.TryParse(value, out var commandSwitch))
                {
                    string commandValue = commandSwitch.Value ?? "";
                    m_Args[commandSwitch.Name] = commandValue;
                }
            }
        }

        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            return m_Args.TryGetValue(name, out value);
        }

    }
}
