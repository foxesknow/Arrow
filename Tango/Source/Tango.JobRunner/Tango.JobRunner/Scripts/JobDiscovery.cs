using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Scripts
{
    static class JobDiscovery
    {
        private static readonly Dictionary<string, Type> s_JobTypes = new(StringComparer.OrdinalIgnoreCase);

        static JobDiscovery()
        {
            // Register any standard jobs to save users having to do so explicitly
            Register(typeof(JobDiscovery).Assembly);
        }

        public static void Register(Assembly assembly)
        {
            LoadFromAssembly(assembly, (type, attribute) => s_JobTypes[attribute.Name] = type);
        }

        public static void LoadFromAssembly(Assembly assembly, Action<Type, JobAttribute> register)
        {
            var types = assembly.GetTypes();

            foreach(var type in types)
            {
                if(type.IsPublic && type.IsClass && type.IsAbstract == false && type.IsGenericType == false)
                {
                    // Well, it's eligable...
                    var attributes = type.GetCustomAttributes<JobAttribute>(false);
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
    }
}
