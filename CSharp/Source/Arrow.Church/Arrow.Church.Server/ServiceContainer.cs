using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Common.Data;
using Arrow.Logging;

namespace Arrow.Church.Server
{
	public partial class ServiceContainer : IDisposable
	{
		private static readonly ILog Log=LogManager.GetDefaultLog();

		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<string,ServiceData> m_Services=new Dictionary<string,ServiceData>();
		
		private readonly ServiceHost m_ServiceHost;

		private long m_Running;

		internal ServiceContainer(ServiceHost serviceHost)
		{
			m_ServiceHost=serviceHost;
		}

		public bool IsRunning
		{
			get{return Interlocked.Read(ref m_Running)==1;}
		}

		public void Add(ChurchService service)
		{
			if(service==null) throw new ArgumentNullException("service");

			// We need to work out the name...
			Type type=service.ServiceInterface;
			var attr=type.GetCustomAttribute<ChurchServiceAttribute>();
			if(attr==null)
			{
				throw new ChurchException("could not infer service name for "+type.ToString());
			}
			
			Add(attr.ServiceName,service);
		}

		public void Add(string serviceName, ChurchService service)
		{
			if(this.IsRunning) throw new InvalidOperationException("ServiceContainer.Add - cannot add once container is started");

			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException("serviceName");
			if(service==null) throw new ArgumentNullException("service");

			var serviceData=CreateServiceData(serviceName,service);
			Log.InfoFormat("ServiceContainer.Add - registering service {0} with interface {1}",serviceName,service.ServiceInterface.Name);

			lock(m_SyncRoot)
			{
				if(m_Services.ContainsKey(serviceName)) throw new ChurchException("service already added: "+serviceName);
				
				m_Services.Add(serviceName,serviceData);
			}
		}

		internal void Start()
		{
			if(Interlocked.CompareExchange(ref m_Running,1,0)==0)
			{
				foreach(var serviceData in m_Services.Values)
				{
					var host=new Host(m_ServiceHost,serviceData);

					var service=serviceData.Service;
					service.ContainerStart(host);

					var serviceStartup=service as IServiceStartup;
					if(serviceStartup!=null) 
                    {
                        var identifier=new ServiceNameIdentifier()
                        {
                            Name=serviceData.ServiceName,
                            Endpoint=host.Endpoint
                        };
                        serviceStartup.Start(identifier);
                    }
				}

				// That's everything started, now let them know
				foreach(var serviceData in m_Services.Values)
				{
					var serviceStartup=serviceData.Service as IServiceStartup;
					if(serviceStartup!=null) serviceStartup.AllStarted();
				}
			}
		}

		internal void Stop()
		{
			if(Interlocked.CompareExchange(ref m_Running,0,1)==1)
			{
				foreach(var serviceData in m_Services.Values)
				{
					var service=serviceData.Service;
					service.ContainerStop();

					var serviceShutdown=service as IServiceShutdown;
					if(serviceShutdown!=null) serviceShutdown.Shutdown();
				}

				// That's everything started, now let them know
				foreach(var serviceData in m_Services.Values)
				{
					var serviceShutdown=serviceData.Service as IServiceShutdown;
					if(serviceShutdown!=null) serviceShutdown.AllShutdown();
				}
			}
		}

		internal bool TryGetServiceData(string serviceName, out ServiceData serviceData)
		{
			if(serviceName==null) throw new ArgumentNullException("serviceName");

			lock(m_SyncRoot)
			{
				return m_Services.TryGetValue(serviceName,out serviceData);
			}
		}

        /// <summary>
        /// Returns the names of all the registered services
        /// </summary>
        /// <returns></returns>
        public List<string> ServiceNames()
        {
            lock(m_SyncRoot)
            {
                return m_Services.Keys.ToList();
            }
        }

		/// <summary>
		/// Attempts to find a service within the current container
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <returns>All the services which implement the interface</returns>
		public IList<TInterface> DiscoverAll<TInterface>() where TInterface:class
		{
			var interfaces=new List<TInterface>();

			lock(m_SyncRoot)
			{
				foreach(var serviceData in m_Services.Values)
				{
					if(serviceData.HasServiceInterface(typeof(TInterface)))
					{
						interfaces.Add(serviceData.Service as TInterface);
					}
				}
			}

			return interfaces;
		}

		public bool TryDiscover<TInterface>(string serviceName, out TInterface instance) where TInterface:class
		{
			lock(m_SyncRoot)
			{
				ServiceData serviceData=null;
				if(m_Services.TryGetValue(serviceName,out serviceData) && serviceData.HasServiceInterface(typeof(TInterface)))
				{
					instance=serviceData.Service as TInterface;
					return true;
				}
				else
				{
					instance=null;
					return false;
				}
			}
		}

		/// <summary>
		/// Executes a service method
		/// </summary>
		/// <param name="serviceName"></param>
		/// <param name="method"></param>
		/// <param name="argument"></param>
		/// <returns></returns>
		public Task<object> Execute(string serviceName, string method, object argument)
		{
			ServiceData serviceData=null;

			lock(m_SyncRoot)
			{
				m_Services.TryGetValue(serviceName,out serviceData);
			}

			/*
			 * It's possible for Execute() to run synchronously
			 * Therefore we need to deal with exceptions thrown here in addition
			 * to in the AfterServiceCall methods
			 */
			try
			{
				if(serviceData!=null)
				{
					return serviceData.Execute(method,argument);
				}
				else
				{
					string message=string.Format("serivce ({0}) not found when asked to call ({1})",serviceName,method);
					throw new ChurchException(message);
				}
			}
			catch(Exception e)
			{
				var source=new TaskCompletionSource<object>();
				source.SetException(e);

				return source.Task;
			}
		}

		private ServiceData CreateServiceData(string serviceName, ChurchService service)
		{
			var serviceData=new ServiceData(serviceName,service);

			var serviceInterface=service.ServiceInterface;

			foreach(var method in serviceInterface.GetMethods())
			{
				var serviceMethod=CreateServiceMethod(serviceInterface,method);
						
				Type returnType, parameterType;
				ExtractMethodTypes(method,out returnType,out parameterType);
				serviceData.AddMethod(method.Name,serviceMethod,returnType,parameterType);
			}

			return serviceData;
		}

		private void ExtractMethodTypes(MethodInfo method, out Type returnType, out Type parameterType)
		{
			returnType=typeof(void);
			parameterType=typeof(void);

			var parameters=method.GetParameters();
			if(parameters.Length!=0) parameterType=parameters[0].ParameterType;

			if(method.ReturnType.IsGenericType)
			{
				returnType=method.ReturnType.GenericTypeArguments[0];
			}
		}
		
		private ServiceMethod CreateServiceMethod(Type @interface, MethodInfo method)
		{
			var parameters=method.GetParameters();
			if(parameters.Length>1) throw new ChurchException("methods can only take zero or one parameters");

			var returnType=method.ReturnType;
			if(typeof(Task).IsAssignableFrom(returnType)==false) throw new ChurchException("return type must be a task");

			var instance=Expression.Parameter(typeof(ChurchService));
			var parameter=Expression.Parameter(typeof(object));

			List<Expression> arguments=new List<Expression>();
			if(parameters.Length==1) arguments.Add(Expression.Convert(parameter,parameters[0].ParameterType));

			// This is the call to the actual method
			var call=Expression.Call
			(
				Expression.Convert(instance,@interface),
				method,
				arguments
			);

			Expression wrappedCall=null;

			// We need to map void return types to a generic task
			// in order to simplify the interface to the methods
			if(returnType.IsGenericType)
			{
				Type taskType=returnType.GenericTypeArguments[0];
				var wrapper=typeof(ServiceContainer).GetMethod("GenericAfterServiceMethod",BindingFlags.Static|BindingFlags.NonPublic).MakeGenericMethod(taskType);
				wrappedCall=Expression.Call(null,wrapper,call);
			}
			else
			{
				var wrapper=typeof(ServiceContainer).GetMethod("AfterServiceMethod",BindingFlags.Static|BindingFlags.NonPublic);
				wrappedCall=Expression.Call(null,wrapper,call);
			}

			var lambda=Expression.Lambda<ServiceMethod>(wrappedCall,instance,parameter);
			return lambda.Compile();
		}


		/// <summary>
		/// Called after a task that returns a value finishes
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="task"></param>
		/// <returns></returns>
		private static Task<object> GenericAfterServiceMethod<T>(Task<T> task)
		{
			TaskCompletionSource<object> source=new TaskCompletionSource<object>();
			
			task.ContinueWith(t=>
			{
				if(t.Status==TaskStatus.Faulted)
				{
					source.SetException(t.Exception);
				}
				else
				{
					source.SetResult(t.Result);
				}
			});

			return source.Task;
		}

		/// <summary>
		/// Called after a task that returns nothing finishes
		/// </summary>
		/// <param name="task"></param>
		/// <returns></returns>
		private static Task<object> AfterServiceMethod(Task task)
		{
			TaskCompletionSource<object> source=new TaskCompletionSource<object>();
			
			task.ContinueWith(t=>
			{			
				if(t.Status==TaskStatus.Faulted)
				{
					source.SetException(t.Exception);
				}
				else
				{
					source.SetResult(VoidValue.Instance);
				}
			});

			return source.Task;
		}

		public void Dispose()
		{
			Stop();

			foreach(var serviceData in m_Services.Values)
			{
				var service=serviceData.Service;

				var disposable=service as IDisposable;
				if(disposable!=null) disposable.Dispose();

				service.ContainerStop();
			}
		}

	}
}
