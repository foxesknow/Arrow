using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tango.JobRunner.Scripts
{
    public sealed class JobFactory
    {
        private readonly Dictionary<string, Type> m_JobTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

        private readonly Func<Type, Job> m_MakeJob;

        /// <summary>
        /// Initializes the instance.
        /// Jobs will be creates via a call to Activator.CreateInstance
        /// </summary>
        public JobFactory() : this(MakeJob)
        {
        }

        /// <summary>
        /// Initializes the instance
        /// </summary>
        /// <param name="makeJob">A function responsible for creating instance of jobs</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JobFactory(Func<Type, Job> makeJob)
        {
            if(makeJob is null) throw new ArgumentNullException(nameof(makeJob));

            m_MakeJob = makeJob;

            foreach(var (name, type) in JobDiscovery.PredefinedJobs)
            {
                Register(name, type);
            }
        }

        public void Register(Assembly assembly)
        {
            if(assembly is null) throw new ArgumentNullException(nameof(assembly));

            JobDiscovery.LoadFromAssembly<JobAttribute>(assembly, (type, attribute) => Register(attribute.Name, type));
        }

        public void Register(string name, Type type)
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

        /// <summary>
        /// Creates a job with the specified name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JobRunnerException"></exception>
        public Job Make(string name)
        {
            if(name is null) throw new ArgumentNullException(nameof(name));

            if(m_JobTypes.TryGetValue(name, out var type))
            {
                return m_MakeJob(type);
            }
            else
            {
                throw new JobRunnerException($"could not find {name}");
            }
        }

        private static Job MakeJob(Type type)
        {
            var instance = Activator.CreateInstance(type);
            if(instance is null) throw new JobRunnerException($"could not create instance of {type.Name}");

            return (Job)instance;
        }
    }
}
