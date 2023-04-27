using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Arrow.Collections;

namespace Arrow.Settings
{
    /// <summary>
    /// Manages a collection of settings which are are assigned to a namespace
    /// NOTE: Namespace names are not case sensitive
    /// 
    /// The "App" namespace is reserved for the hosting process
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>
        /// The character used to separate a namespace from the setting name
        /// </summary>
        public static readonly string NamespaceSeparator = ":";

        /// <summary>
        /// The character used to separate a namespace from the setting name
        /// </summary>
        public static readonly char NamespaceSeparatorChar = ':';

        private static readonly char[] s_NamespaceSeparatorCharArray = new char[]{NamespaceSeparatorChar};

        private static readonly object s_Lock = new object();

        private static readonly Dictionary<string, ISettings> s_Namespaces = new Dictionary<string, ISettings>(StringComparer.OrdinalIgnoreCase);
        private static readonly LinkedList<string> s_NameStack = new LinkedList<string>();

        static SettingsManager()
        {
            DoRegister("guid", GuidSettings.Instance);
            DoRegister("env", EnvironmentSettings.Instance);
            DoRegister("dotnetenv", DotNetSettings.Instance);
            DoRegister("proc", ProcessSettings.Instance);
            DoRegister("datetime", DateTimeSettings.Instance);
            DoRegister("net", NetworkSettings.Instance);
            DoRegister("fs", FileSystemSettings.Instance);
            DoRegister("cmdline", CommandLineSettings.Instance);

            // These settings aren't stacked
            DoRegister("def", DefaultSettings.Instance, false);
            DoRegister("any", AnySettings.Instance, false);
            DoRegister("appsettings", new AppConfigSettings(), false);
            DoRegister("first-of", new FirstOfSettings(), false);
            DoRegister("string", new StringSettings(), false);
            DoRegister("slurp", new SlurpSettings(), false);
            DoRegister("literal", new LiteralSettings(), false);
            DoRegister("aesdecrypt", new AesEncryptionSettings(), false);


            // Load any registered setting providers
            Arrow.Settings.Config.SettingProvidersProcessAppConfig.Process();
        }

        /// <summary>
        /// Registers a new setting. 
        /// If the namespace already exists the setting is aggregated with the existing setting
        /// </summary>
        /// <param name="namespace">The namespace to register</param>
        /// <param name="settings">The provider to register</param>
        /// <exception cref="System.ArgumentNullException">namespace is null</exception>
        /// <exception cref="System.ArgumentNullException">settings is null</exception>
        public static void Register(string @namespace, ISettings settings)
        {
            if(@namespace == null) throw new ArgumentNullException("namespace");
            if(settings == null) throw new ArgumentNullException("settings");

            DoRegister(@namespace, settings);
        }

        /// <summary>
        /// Checks to see if a namespace is already registered
        /// </summary>
        /// <param name="namespace">The namespace to check</param>
        /// <returns>true if the namespace is registred, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">namespace is null</exception>
        public static bool IsRegistered(string @namespace)
        {
            if(@namespace == null) throw new ArgumentNullException("namespace");

            lock(s_Lock)
            {
                return s_Namespaces.ContainsKey(@namespace);
            }
        }

        /// <summary>
        /// Returns the settings registered to a namespace
        /// </summary>
        /// <param name="namespace">The namespace to look up</param>
        /// <returns>The settings in the namespace, or null if one does not exist</returns>
        /// <exception cref="System.ArgumentNullException">namespace is null</exception>
        public static ISettings? GetSettings(string @namespace)
        {
            if(@namespace == null) throw new ArgumentNullException("namespace");

            lock(s_Lock)
            {
                s_Namespaces.TryGetValue(@namespace, out var settings);
                return settings;
            }
        }

        /// <summary>
        /// Returns all the namespaces that are registered
        /// </summary>
        /// <value>A list of namespace names</value>
        public static IReadOnlyList<string> Namespaces
        {
            get
            {
                lock(s_Lock)
                {
                    return new List<string>(s_Namespaces.Keys);
                }
            }
        }

        /// <summary>
        /// Returns the namespaces names as a list
        /// with the most recently added namespaces near the beginning of the list
        /// </summary>
        public static IReadOnlyList<string> NamespaceStack
        {
            get
            {
                lock(s_Lock)
                {
                    return new List<string>(s_NameStack);
                }
            }
        }

        /// <summary>
        /// Splits a qualified name into the setting parts (setting namespace and setting name).
        /// Throws an exception if the scoped name is invalid
        /// </summary>
        /// <param name="qualifiedName">The name of the setting</param>
        /// <param name="namespace">The namespace part of the scoped name</param>
        /// <param name="setting">The setting part of the scoped name</param>
        public static void CrackQualifiedName(string qualifiedName, out string @namespace, out string setting)
        {
            var parts = qualifiedName.Split(s_NamespaceSeparatorCharArray, 2);
            if(parts.Length != 2) throw new ArgumentException("qualifiedName not in the format namespace:settingName");

            @namespace = parts[0];
            setting = parts[1];
        }

        /// <summary>
        /// Creates a qualied name for a setting
        /// </summary>
        /// <param name="namespace">The namespace of the setting</param>
        /// <param name="setting">The name of the setting</param>
        /// <returns>A scoped name</returns>
        /// <exception cref="System.ArgumentNullException">namespace is null</exception>
        /// <exception cref="System.ArgumentNullException">name is null</exception>
        public static string MakeQualifiedName(string @namespace, string setting)
        {
            if(@namespace == null) throw new ArgumentNullException("namespace");
            if(setting == null) throw new ArgumentNullException("setting");

            string scopedName = string.Format("{0}{1}{2}", @namespace, NamespaceSeparatorChar, setting);
            return scopedName;
        }

        /// <summary>
        /// Determines if a name represents a qualified setting name
        /// </summary>
        /// <param name="name">The name to check</param>
        /// <returns>true if name is a qualified name, false otherwise</returns>
        public static bool IsQualifiedname(string name)
        {
            return name != null && name.IndexOf(NamespaceSeparatorChar) != -1;
        }


        /// <summary>
        /// Registers a namespace and store its name in the namespace stack
        /// </summary>
        /// <param name="namespace">The namespace to register</param>
        /// <param name="settings">The provider to register</param>
        private static void DoRegister(string @namespace, ISettings settings)
        {
            DoRegister(@namespace, settings, true);
        }

        /// <summary>
        /// Get a a setting from a qualified name in the format namespace:setting
        /// or throws an exception if the setting does not exist
        /// </summary>
        /// <typeparam name="T">The type to cast the setting to, usually a string</typeparam>
        /// <param name="qualifiedName">The name of the setting</param>
        /// <returns>The setting for the qualified name</returns>
        public static T Setting<T>(string qualifiedName)
        {
            T value;
            if(TryGetSetting(qualifiedName, out value) == false)
            {
                throw new ArrowException("setting not found: " + qualifiedName);
            }

            return value;
        }

        /// <summary>
        /// Get a a setting from a scoped name in the format namespace:setting
        /// or returns defaultValue if it does not exist
        /// </summary>
        /// <typeparam name="T">The type to cast the setting to (typically a string)</typeparam>
        /// <param name="qualifiedName">The name of the setting</param>
        /// <param name="defaultValue">The value to return if the setting could not be found</param>
        /// <returns>The value of the setting</returns>
        /// <exception cref="System.ArgumentNullException">scopedName is null</exception>
        public static T Setting<T>(string qualifiedName, T defaultValue)
        {
            if(TryGetSetting<T>(qualifiedName, out var value) == false)
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// Attempts to get a a setting from a qualified name in the format namespace:setting
        /// </summary>
        /// <typeparam name="T">The type to cast the setting to (typically a string)</typeparam>
        /// <param name="qualifiedName">The name of the setting</param>
        /// <param name="value">On return the value of the setting if found, otherwise the default value for the type</param>
        /// <returns>true if the setting was found, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">scopedName is null</exception>
        public static bool TryGetSetting<T>(string qualifiedName, out T value)
        {
            if(qualifiedName == null) throw new ArgumentNullException("qualifiedName");

            CrackQualifiedName(qualifiedName, out var @namespace, out var setting);

            return TryGetSetting(@namespace, setting, out value);
        }

        /// <summary>
        /// Registers a namespace and store its name in the namespace stack
        /// </summary>
        /// <param name="namespace">The namespace to register</param>
        /// <param name="settings">The provider to register</param>
        /// <param name="addToStack">true to store the namespace in the stack, false to leave it out</param>
        private static void DoRegister(string @namespace, ISettings settings, bool addToStack)
        {
            lock(s_Lock)
            {
                if(s_Namespaces.TryGetValue(@namespace, out var existingSettings))
                {
                    // We'll combine them, with the newest getting priority
                    var cons = new ConsSetting(settings, existingSettings);
                    s_Namespaces[@namespace] = cons;
                }
                else
                {
                    s_Namespaces[@namespace] = settings;
                    if(addToStack) s_NameStack.AddFirst(@namespace);
                }
            }
        }

        /// <summary>
        /// Attempts to get a setting value from a particular namespace
        /// </summary>
        /// <typeparam name="T">The type to cast the setting to (typically a string)</typeparam>
        /// <param name="namespace">The namespace to get the setting from</param>
        /// <param name="setting">The name of the setting to get</param>
        /// <param name="value">On return the value of the setting if found, otherwise the default value for the type</param>
        /// <returns>true if the setting was found, false otherwise</returns>
        /// <exception cref="System.ArgumentNullException">namespace is null</exception>
        /// <exception cref="System.ArgumentNullException">setting is null</exception>
        private static bool TryGetSetting<T>(string @namespace, string setting, out T value)
        {
            if(@namespace == null) throw new ArgumentNullException("namespace");
            if(setting == null) throw new ArgumentNullException("setting");

            value = default!;

            bool success = false;
            var settings = GetSettings(@namespace);
            if(settings != null)
            {
                if(settings.TryGetSetting(setting, out var o))
                {
                    // The actual setting may be a boxed type (like an int).
                    // In this case if T is a long the cast will fail as unboxing requires the correct type.
                    // Therefore we need to check type assignment and do a conversion if necessary
                    if(typeof(T).IsAssignableFrom(o.GetType()))
                    {
                        value = (T)o;
                    }
                    else
                    {
                        value = (T)Convert.ChangeType(o, typeof(T));
                    }

                    success = true;
                }
            }

            return success;
        }
    }
}
