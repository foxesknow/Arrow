using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Arrow.Logging
{
    /// <summary>
    /// Active properties are generated each time the property is sent to a og file.
    /// An active property is a string->object where the string is the name of the property
    /// </summary>
    public sealed class ActiveProperties
    {
        /// <summary>
        /// Expands a value, taking into account it may be an active property
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object? Expand(string name, object? value)
        {
            if(IsActiveProperty(value, out var function))
            {
                return function(name);
            }

            return value;
        }

        /// <summary>
        /// Checks to see if a value is an active property
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        public static bool IsActiveProperty(object? value, [NotNullWhen(true)] out Func<string, object?>? function)
        {
            function = value as Func<string, object?>;
            return function is not null;
        }
    }
}
