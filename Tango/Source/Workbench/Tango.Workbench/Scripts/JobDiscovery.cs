using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Scripts
{
    static class JobDiscovery
    {
        private static readonly Dictionary<string, Type> s_JobTypes = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Type> s_SourceTypes = new(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, Type> s_FilterTypes = new(StringComparer.OrdinalIgnoreCase);

        static JobDiscovery()
        {
            // Register any standard jobs to save users having to do so explicitly
            Register(typeof(JobDiscovery).Assembly);
        }

        public static void Register(Assembly assembly)
        {
            LoadFromAssembly<JobAttribute>(assembly, (type, attribute) => s_JobTypes[attribute.Name] = type);
            LoadFromAssembly<SourceAttribute>(assembly, (type, attribute) => s_SourceTypes[attribute.Name] = type);
            LoadFromAssembly<FilterAttribute>(assembly, (type, attribute) => s_FilterTypes[attribute.Name] = type);
        }

        public static void LoadFromAssembly<TAttribute>(Assembly assembly, Action<Type, TAttribute> register) where TAttribute : Attribute
        {
            var types = assembly.GetTypes();

            foreach(var type in types)
            {
                if(type.IsPublic && type.IsClass && type.IsAbstract == false && type.IsGenericType == false)
                {
                    // Well, it's eligable...
                    var attributes = type.GetCustomAttributes<TAttribute>(false);
                    foreach(var attribute in attributes)
                    {
                        register(type, attribute);
                    }
                }
            }
        }

        public static IReadOnlyDictionary<string, Type> PredefinedJobs
        {
            get{return s_JobTypes;}
        }

        public static IReadOnlyDictionary<string, Type> PredefinedSources
        {
            get{return s_SourceTypes;}
        }

        public static IReadOnlyDictionary<string, Type> PredefinedFilters
        {
            get{return s_FilterTypes;}
        }
    }
}
