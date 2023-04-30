using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tango.Workbench.Scripts
{
    /// <summary>
    /// The class is responsible for creating job instnaces
    /// </summary>
    public sealed class JobFactory
    {
        private readonly Dictionary<string, Type> m_JobTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Type> m_SourceTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, Type> m_FilterTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private readonly Func<Type, object> m_MakeComponent;

        /// <summary>
        /// Initializes the instance.
        /// Jobs will be creates via a call to Activator.CreateInstance
        /// </summary>
        public JobFactory() : this(MakeComponent)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="makeJob">A function responsible for creating instance of jobs</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JobFactory(Func<Type, object> makeComponent)
        {
            if(makeComponent is null) throw new ArgumentNullException(nameof(makeComponent));

            m_MakeComponent = makeComponent;

            foreach(var (name, type) in JobDiscovery.PredefinedJobs)
            {
                RegisterJob(name, type);
            }

            foreach(var (name, type) in JobDiscovery.PredefinedSources)
            {
                RegisterSource(name, type);
            }

            foreach(var (name, type) in JobDiscovery.PredefinedFilters)
            {
                RegisterFilter(name, type);
            }
        }

        /// <summary>
        /// Registers all jobs in an assembly
        /// </summary>
        /// <param name="assembly"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Register(Assembly assembly)
        {
            if(assembly is null) throw new ArgumentNullException(nameof(assembly));

            JobDiscovery.LoadFromAssembly<JobAttribute>(assembly, (type, attribute) => RegisterJob(attribute.Name, type));
            JobDiscovery.LoadFromAssembly<SourceAttribute>(assembly, (type, attribute) => RegisterSource(attribute.Name, type));
            JobDiscovery.LoadFromAssembly<FilterAttribute>(assembly, (type, attribute) => RegisterFilter(attribute.Name, type));
        }

        /// <summary>
        /// Explicitly registers a job with the given name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public void RegisterJob(string name, Type type)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));
            if(type is null) throw new ArgumentNullException(nameof(type));

            if(typeof(Job).IsAssignableFrom(type))
            {
                m_JobTypes[name] = type;
            }
            else
            {
                throw new ArgumentException($"{type.Name} is not a job");
            }
        }

        public void RegisterSource(string name, Type type)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));
            if(type is null) throw new ArgumentNullException(nameof(type));

            if(typeof(Source).IsAssignableFrom(type))
            {
                m_SourceTypes[name] = type;
            }
            else
            {
                throw new ArgumentException($"{type.Name} is not a source");
            }
        }

        public void RegisterFilter(string name, Type type)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));
            if(string.IsNullOrWhiteSpace(name)) throw new ArgumentException("name is empty", nameof(name));
            if(type is null) throw new ArgumentNullException(nameof(type));

            if(typeof(Filter).IsAssignableFrom(type))
            {
                m_FilterTypes[name] = type;
            }
            else
            {
                throw new ArgumentException($"{type.Name} is not a dilter");
            }
        }

        /// <summary>
        /// Creates a job with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="WorkbenchException"></exception>
        public Job MakeJob(string name)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));

            if(m_JobTypes.TryGetValue(name, out var type))
            {
                return (Job)m_MakeComponent(type);
            }
            else
            {
                throw new WorkbenchException($"could not find {name}");
            }
        }

        public Source MakeSource(string name)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));

            if(m_SourceTypes.TryGetValue(name, out var type))
            {
                return (Source)m_MakeComponent(type);
            }
            else
            {
                throw new WorkbenchException($"could not find {name}");
            }
        }

        public Filter MakeFilter(string name)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));

            if(m_FilterTypes.TryGetValue(name, out var type))
            {
                return (Filter)m_MakeComponent(type);
            }
            else
            {
                throw new WorkbenchException($"could not find {name}");
            }
        }

        /// <summary>
        /// The default component creator, which uses Activator.CreateInstance.
        /// Callers can specify their own creator if they want to use DI
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="WorkbenchException"></exception>
        private static object MakeComponent(Type type)
        {
            var instance = Activator.CreateInstance(type);
            if(instance is null) throw new WorkbenchException($"could not create instance of {type.Name}");

            return instance;
        }
    }
}
