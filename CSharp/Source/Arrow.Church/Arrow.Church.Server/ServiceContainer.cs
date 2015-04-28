using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Arrow.Church.Common;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Server
{
	public class ServiceContainer
	{
		private readonly object m_SyncRoot=new object();
		private readonly Dictionary<string,ServiceData> m_Services=new Dictionary<string,ServiceData>();

		public void Add(string serviceName, ChurchService service)
		{
			if(serviceName==null) throw new ArgumentNullException("serviceName");
			if(string.IsNullOrWhiteSpace(serviceName)) throw new ArgumentException("serviceName");
			if(service==null) throw new ArgumentNullException("service");

			var serviceData=CreateServiceData(service);

			lock(m_SyncRoot)
			{
				m_Services.Add(serviceName,serviceData);
			}
		}

		internal bool TryGetServiceData(string serviceName, out ServiceData serviceData)
		{
			lock(m_SyncRoot)
			{
				return m_Services.TryGetValue(serviceName,out serviceData);
			}
		}

		public TInterface Discover<TInterface>() where TInterface:class
		{
			TInterface @interface=null;

			if(TryDiscover<TInterface>(out @interface))
			{
				return @interface;
			}
			else
			{
				throw new ServiceNotFoundException("could not find a service that implements: "+typeof(TInterface).Name);
			}
		}

		public bool TryDiscover<TInterface>(out TInterface instance) where TInterface:class
		{
			ServiceData serviceData=null;

			lock(m_SyncRoot)
			{
				serviceData=m_Services.Values.FirstOrDefault(s=>s.HasInterface(typeof(TInterface)));
			}

			if(serviceData!=null)
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

		private ServiceData CreateServiceData(ChurchService service)
		{
			var serviceData=new ServiceData(service);

			Type type=service.GetType();
			
			foreach(Type @interface in type.GetInterfaces())
			{
				if(@interface.IsDefined(typeof(ChurchServiceAttribute)))
				{
					foreach(var method in @interface.GetMethods())
					{
						var serviceMethod=CreateServiceMethod(@interface,method);
						
						Type returnType, parameterType;
						ExtractMethodTypes(method,out returnType,out parameterType);
						serviceData.AddMethod(method.Name,serviceMethod,returnType,parameterType);
					}

					serviceData.AddInterface(@interface);
				}
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
	}
}
