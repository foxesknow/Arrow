using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;

using Arrow.Configuration;

namespace Arrow.Factory
{
    /// <summary>
    /// Scans an assembly looking for types that have the RegisteredType attribute and registers them with the appropriate factory.
    /// The class tracks which assemblies are passed to it so that it will only every scan an assembly once.
    /// </summary>
    public static class RegisteredTypeInstaller
    {
        private static readonly object s_SyncRoot = new object();

        private static readonly Dictionary<Assembly, bool> s_AssembliesProcessed = new Dictionary<Assembly, bool>();

        private static readonly Dictionary<Type, MethodInfo> s_FactoryRegister = new Dictionary<Type, MethodInfo>();

        /// <summary>
        /// Registers assemblies from the <b>Arrow/Arrow.Factory/RegisteredTypeInstaller</b> section of the app config file.
        /// Each assembly to load must be in an <b>Assembly</b> element.
        /// </summary>
        public static void LoadTypesFromAppConfig()
        {
            var installerNode = AppConfig.GetSectionXml(ArrowSystem.Name, "Arrow.Factory/RegisteredTypeInstaller");
            if(installerNode != null)
            {
                RegisteredTypeInstallerConfiguration config = RegisteredTypeInstallerConfiguration.FromXml(installerNode);
                config.Apply();
            }
        }

        /// <summary>
        /// Loads and registers all public non-abstract classes in the assembly that have a RegisteredType attribute attached to them
        /// </summary>
        /// <param name="assemblyFilename">The filename to load and scan</param>
        /// <returns>The assembly that was loaded and scanned</returns>
        public static Assembly? LoadTypes(string assemblyFilename)
        {
            var assembly = LoadAssembly(assemblyFilename);

            if(assembly != null)
            {
                LoadTypes(assembly);
            }
            return assembly;
        }

        /// <summary>
        /// Loads and registers all public non-abstract classes in the assembly that have a RegisteredType attribute attached to them
        /// </summary>
        /// <param name="assembly">The assembly to scan</param>
        /// <exception cref="System.ArgumentNullException">assembly is null</exception>
        public static void LoadTypes(Assembly assembly)
        {
            if(assembly == null) throw new ArgumentNullException("assembly");

            lock(s_SyncRoot)
            {
                if(s_AssembliesProcessed.ContainsKey(assembly))
                {
                    // No need to do it twice!
                    return; // NOTE: Early return
                }

                s_AssembliesProcessed[assembly] = true;

                Type[] types = assembly.GetTypes();
                foreach(Type type in types)
                {
                    if(type.IsClass && type.IsPublic && !type.IsAbstract)
                    {
                        object[] attributes = type.GetCustomAttributes(typeof(RegisteredTypeAttribute), false);
                        foreach(RegisteredTypeAttribute attribute in attributes)
                        {
                            RegisterType(type, attribute);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Registers the actual type with the appropriate factory
        /// </summary>
        /// <param name="type">The type to register</param>
        /// <param name="attribute">The registration attribute attached to the type</param>
        private static void RegisterType(Type type, RegisteredTypeAttribute attribute)
        {
            Type factory = attribute.FactoryType;
            string name = attribute.Name;

            MethodInfo? registration = null;
            s_FactoryRegister.TryGetValue(factory, out registration);
            if(registration == null)
            {
                // We need to get a handle to the static registration method on the class
                registration = factory.GetMethod("Register", new Type[] { typeof(string), typeof(Type) });
                if(registration == null)
                {
                    throw new ApplicationException("Could not find Register method in " + factory.ToString());
                }

                s_FactoryRegister[factory] = registration;

            }

            registration.Invoke(null, new object[] { name, type });
        }

        /// <summary>
        /// Loads an assembly
        /// </summary>
        /// <param name="assemblyName">The assembly to load</param>
        private static Assembly? LoadAssembly(string assemblyName)
        {
            try
            {
                Assembly assembly = Assembly.Load(assemblyName);
                return assembly;
            }
            catch(FileNotFoundException)
            {
                // There's no progmatic way to detect the absence of 
                // an assembly, so we'll need to catch the exception
                return null;
            }
            catch(FileLoadException)
            {
                // Bizarrly, this exception can also be thrown when
                // an assembly cannot be located
                return null;
            }
        }
    }
}
