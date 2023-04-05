using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        private static Guid s_AppGuid = Guid.NewGuid();

        /// <summary>
        /// An instance of the class that may be shared
        /// </summary>
        public static readonly ISettings Instance = new GuidSettings();

        /// <inheritdoc/>
        public bool TryGetSetting(string name, [NotNullWhen(true)] out object? value)
        {
            switch(name.ToLower())
            {
                case "app":
                    value = s_AppGuid;
                    return true;

                case "new":
                    value = Guid.NewGuid();
                    return true;

                default:
                    value = null;
                    return false;
            }
        }
    }
}
