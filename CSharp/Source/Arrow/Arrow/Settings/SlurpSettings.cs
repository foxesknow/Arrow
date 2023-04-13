using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.IO;

namespace Arrow.Settings
{
    /// <summary>
    /// Treats the setting as the name of a file to load.
    /// The contents of the file become the value of the setting.
    /// 
    /// If the filename ends with a ? then it is allowed to be missing, and the value will be set to null.
    /// Otherwise an IOException is thrown
    /// </summary>
    public sealed class SlurpSettings : ISettings
    {
        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            var setting = Parse(name);

            if(File.Exists(setting.Filename))
            {
                value = File.ReadAllText(setting.Filename);
                return true;
            }

            if(setting.IsOptional)
            {
                value = null;
                return false;
            }

            throw new IOException($"could not find file {setting.Filename}");
        }

        private static (bool IsOptional, string Filename) Parse(string setting)
        {
            if(setting.EndsWith("?"))
            {
                var filename = setting.Substring(0, setting.Length - 1);
                return (true, filename);
            }

            return (false, setting);
        }
    }
}
