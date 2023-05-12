using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Arrow.Settings;

namespace Arrow.Text
{
    /// <summary>
    /// Expands text which may contain tokens
    /// </summary>
    public static class TokenExpander
    {
        private static readonly char[] TokenPipelineSeparator = new char[]{'|'};

        /// <summary>
        /// A default begin token to use
        /// </summary>
        public static readonly string DefaultBeginToken = "${";

        /// <summary>
        /// A default end token to use
        /// </summary>
        public static readonly string DefaultEndToken = "}";

        private static readonly BindingFlags PropertyBindings = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance;


        /// <summary>
        /// Expands a string using the default begin and end tokens
        /// </summary>
        /// <param name="value">The string to expand</param>
        /// <returns>An expanded version of the string</returns>
        public static string ExpandText(string value)
        {
            return ExpandText(value, DefaultBeginToken, DefaultEndToken);
        }

        /// <summary>
        /// Expands a string that may contains tokens prefixed by beginMark and terminated by endMark
        /// </summary>
        /// <param name="value">The string to examine</param>
        /// <param name="beginToken">The characters that mark the start of a token</param>
        /// <param name="endToken">The characters that mark the end of the token</param>
        /// <returns>The expanded version of value</returns>
        public static string ExpandText(string value, string beginToken, string endToken)
        {
            return ExpandText(value, beginToken, endToken, null);
        }

        /// <summary>
        /// Expands a string using the default begin and end tokens
        /// </summary>
        /// <param name="value">The string to expand</param>
        /// <param name="unknownVariableLookup">A function to call if the variable cannot be resolved (may be null)</param>
        /// <returns>An expanded version of the string</returns>
        public static string ExpandText(string value, Func<string, object?> unknownVariableLookup)
        {
            return ExpandText(value, DefaultBeginToken, DefaultEndToken, unknownVariableLookup);
        }

        /// <summary>
        /// Expands a string that may contains tokens prefixed by beginMark and terminated by endMark.
        /// If the token cannot be expanded then the optional unknownVariableLookup handler is called
        /// to give the caller the chance to resolve the value
        /// </summary>
        /// <param name="value">The string to examine</param>
        /// <param name="beginToken">The characters that mark the start of a token</param>
        /// <param name="endToken">The characters that mark the end of the token</param>
        /// <param name="unknownVariableLookup">A function to call if the variable cannot be resolved (may be null)</param>
        /// <returns>The expanded version of value</returns>
        public static string ExpandText(string value, string beginToken, string endToken, Func<string, object?>? unknownVariableLookup)
        {
            int beginMarkLength = beginToken.Length;
            int endMarkLength = endToken.Length;

            int index = -1;
            int startIndex = 0;

            while((index = value.IndexOf(beginToken, startIndex)) != -1)
            {
                int end = value.IndexOf(endToken, index + 1);
                if(end == -1)
                {
                    break;
                }
                else
                {
                    string token = value.Substring(index + beginMarkLength, (end - index) - beginMarkLength);
                    string tokenValue = ExpandToken(token, unknownVariableLookup);

                    if(tokenValue == null)
                    {
                        throw new ArrowException($"could not resolve {token}");
                    }

                    string leftPart = value.Substring(0, index);
                    string rightPart = value.Substring(end + endMarkLength);

                    value = leftPart + tokenValue + rightPart;
                    startIndex = leftPart.Length + tokenValue.Length;
                }
            }

            return value;
        }

        /// <summary>
        /// Expands a token of the form namespace:variable|formatting|action
        /// </summary>
        /// <param name="token">The token to expand</param>
        /// <returns>The value for the token</returns>
        public static string ExpandToken(string token)
        {
            return ExpandToken(token, null);
        }

        /// <summary>
        /// Expands a token of the form namespace:variable|property|formatting|action
        /// </summary>
        /// <param name="token">The token to expand</param>
        /// <param name="unknownVariableLookup">The handler to call if the variable is not found or does not have a namespace qualifier</param>
        /// <returns>The value for the token</returns>
        /// <exception cref="System.ArgumentNullException">token is null</exception>
        public static string ExpandToken(string token, Func<string, object?>? unknownVariableLookup)
        {
            if(token == null) throw new ArgumentNullException(nameof(token));            

            string? @namespace = null;            

            // Split the pipeline apart to get the variable|property|formatting|action parts
            string[] parts = token.Split(TokenPipelineSeparator, 4);
            var variable = (parts.Length > 0 ? parts[0] : null);
            var property = (parts.Length > 1 ? parts[1] : null);
            var formatting = (parts.Length > 2 ? parts[2] : null);
            var action = (parts.Length > 3 ? parts[3] : null);

            if(variable == null) throw new ArrowException("token does not contain a variable: " + token);

            string originalVariable = variable;

            // See if the token comes from a namespace
            int pivot = variable.IndexOf(SettingsManager.NamespaceSeparatorChar);

            if(pivot == -1)
            {
                // It's a regular variable
                @namespace = null;
            }
            else
            {
                // It's a value from a namespace
                @namespace = variable.Substring(0, pivot);
                variable = variable.Substring(pivot + 1);
            }

            object? value = null;

            if(@namespace == null)
            {
                // If no namespace pass it to the unknown lookup
                if(unknownVariableLookup != null)
                {
                    value = unknownVariableLookup(originalVariable);
                }
            }
            else
            {
                // Get the variable from the namespace
                ISettings? settings = SettingsManager.GetSettings(@namespace);
                if(settings != null)
                {
                    settings.TryGetSetting(variable, out value);
                }
                else
                {
                    // If it's not a namespace from the global settings then give the caller a change to look up the value
                    if(unknownVariableLookup != null)
                    {
                        value = unknownVariableLookup(originalVariable);
                    }
                }
            }

            // We need a value
            if(value == null) throw new ArrowException($"{token} could not be resolved");

            value = ApplyProperty(property, value);

            string result = ApplyFormatting(formatting, value);
            result = ApplyAction(action, result);

            return result;
        }

        private static object ApplyProperty(string? property, object value)
        {
            if(string.IsNullOrEmpty(property)) return value;

            object? resolvedValue = value;
                
            foreach(string propertyName in property!.Split('.'))
            {
                if(resolvedValue is ISettings)
                {
                    // Since a settings is just a bag of values it makes sense to treat is as effectivly a bunch of properties
                    ISettings settings = (ISettings)resolvedValue;
                    settings.TryGetSetting(propertyName, out resolvedValue);
                }
                else
                {
                    if(resolvedValue is null) throw new ArrowException($"cannot get property {propertyName} on a null instance");

                    var info = resolvedValue.GetType().GetProperty(propertyName, PropertyBindings);
                    if(info == null) throw new ArrowException($"property not found: {propertyName}");

                    var method = info.GetGetMethod();
                    if(method == null) throw new ArrowException($"property not readable: {propertyName}");

                    resolvedValue = method.Invoke(resolvedValue, null);
                }
            }

            if(resolvedValue is null) throw new ArrowException("property lookup of {property} resolved to a null value");

            return resolvedValue;
        }

        private static string ApplyFormatting(string? formatting, object value)
        {
            if(string.IsNullOrEmpty(formatting))
            {
                return value.ToString()!;
            }
            else
            {
                if(value is IFormattable formattable)
                {
                    return formattable.ToString(formatting, null);
                }
                else
                {
                    // As the object isn't formattable just return the value
                    return value.ToString()!;
                }
            }
        }

        private static string ApplyAction(string? action, string value)
        {
            if(string.IsNullOrEmpty(action)) return value;

            return action!.ToLower() switch
            {
                "trim"      => value.Trim(),
                "trimstart" => value.TrimStart(),
                "trimend"   => value.TrimEnd(),
                "toupper"   => value.ToUpper(),
                "tolower"   => value.ToLower(),
                _           => throw new ArrowException($"invalid action {action}")
            };
        }
    }
}
