using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Client.Proxy
{
	partial class ProxyBase
	{
		private static long s_ClassID;
		private static readonly object s_SyncRoot=new object();
		private static readonly Dictionary<Type,ProxyCreator> s_Proxies=new Dictionary<Type,ProxyCreator>();

		public static ProxyFactory GetProxyFactory(Type type)
		{
			if(type==null) throw new ArgumentNullException("type");
			if(type.IsInterface==false) throw new ArgumentException("not an interface: "+type.ToString(),"type");

			lock(s_SyncRoot)
			{
				ProxyCreator proxyFactory=null;
				if(s_Proxies.TryGetValue(type,out proxyFactory)==false)
				{
					proxyFactory=GenerateProxyFactory(type);
					s_Proxies.Add(type,proxyFactory);
				}

				return new ProxyFactory(proxyFactory);
			}
		}

		/// <summary>
		/// Generates a proxy for the specified interface.
		/// The resulting class has a constructor of type (Uri endpoint, ServiceDispatcher dispatcher)
		/// </summary>
		/// <param name="interface"></param>
		/// <returns></returns>
		private static ProxyCreator GenerateProxyFactory(Type @interface)
		{
			var classname=GenerateClassName(@interface);
			var builder=CreateTypeBuilder(classname,@interface);

			ImplementConstructor(builder,@interface);

			var methodNames=new HashSet<string>();

			foreach(var methodInfo in @interface.GetMethods())
			{
				var name=methodInfo.Name;

				if(methodNames.Add(name))
				{
					ImplementMethod(builder,methodInfo);
				}
				else
				{
					string message=string.Format("overloaded method names not support. See {0} on {1}",name,@interface.Name);
					throw new ChurchException(message);
				}
			}

			Type type=builder.CreateType();
			var ctor=type.GetConstructor(new Type[]{typeof(Uri),typeof(string)});

			var endpoint=Expression.Parameter(typeof(Uri));
			var serviceName=Expression.Parameter(typeof(string));
			var @new=Expression.New(ctor,endpoint,serviceName);
			var lambda=Expression.Lambda<ProxyCreator>(@new,endpoint,serviceName);
			
			return lambda.Compile();
		}

		private static void ImplementConstructor(TypeBuilder builder, Type @interface)
		{
			ConstructorInfo baseCtor=typeof(ProxyBase).GetConstructor
			(
				new Type[]
				{
					typeof(Uri),
					typeof(string),
					typeof(MessageProtocol)
				}
			);

			MethodAttributes attr=MethodAttributes.Public|MethodAttributes.HideBySig;
			ConstructorBuilder ctor=builder.DefineConstructor(attr,CallingConventions.Standard,new Type[]{typeof(Uri),typeof(string)});

			Type protcolType=ExtractMessageProtocol(@interface);
			var protocolCtor=protcolType.GetConstructor(Type.EmptyTypes);

			var gen=ctor.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0);				// this
			gen.Emit(OpCodes.Ldarg_1);				// Uri
			gen.Emit(OpCodes.Ldarg_2);				// string (service name)
			gen.Emit(OpCodes.Newobj,protocolCtor);
			gen.Emit(OpCodes.Call,baseCtor);		// The protocol hander

			PopulateMethodInfo(gen,@interface);

			gen.Emit(OpCodes.Ret);
		}

		/// <summary>
		/// Works out the return types of the service methods and calls 
		/// the appropriate proxy method to record them
		/// </summary>
		/// <param name="gen"></param>
		/// <param name="interface"></param>
		private static void PopulateMethodInfo(ILGenerator gen, Type @interface)
		{
			var addReturnType=typeof(ProxyBase).GetMethod("AddReturnType",BindingFlags.Instance|BindingFlags.NonPublic);

			foreach(var method in @interface.GetMethods())
			{
				string methodName=method.Name;
				Type returnType=ExtractTaskReturnType(method.ReturnType);

				gen.Emit(OpCodes.Ldarg_0);
				gen.Emit(OpCodes.Ldstr,methodName);
				gen.Emit(OpCodes.Ldtoken,returnType);
				gen.Emit(OpCodes.Callvirt,addReturnType);
			}
		}

		private static void ImplementMethod(TypeBuilder builder, MethodInfo methodInfo)
		{
			if(methodInfo.IsGenericMethod) throw new ChurchException("cannot proxy to generic method: "+methodInfo.Name);

			var parameters=methodInfo.GetParameters();
			if(parameters.Length>1) throw new ChurchException("methods can only take zero or one parameters");

			MethodAttributes attr=	MethodAttributes.Private|MethodAttributes.Virtual|
									MethodAttributes.Final|MethodAttributes.HideBySig|
									MethodAttributes.SpecialName|MethodAttributes.NewSlot;
			
			string methodName=methodInfo.DeclaringType.FullName+"."+methodInfo.Name;
			MethodBuilder method=builder.DefineMethod(methodName,attr);
			
			Type parameterType=null;

			if(parameters.Length!=0)
			{
				parameterType=parameters[0].ParameterType;
				method.SetParameters(parameterType);
			}
			else
			{
				method.SetParameters(Type.EmptyTypes);
			}
			
			Type returnType=methodInfo.ReturnType;
			method.SetReturnType(returnType);

			var gen=method.GetILGenerator();

			MethodInfo target=null;

			// Work out which method to call
			if(returnType.IsGenericType)
			{
				Type taskType=returnType.GenericTypeArguments[0];
				target=typeof(ProxyBase).GetMethod("GenericCall",BindingFlags.NonPublic|BindingFlags.Instance).MakeGenericMethod(taskType);
			}
			else
			{
				target=typeof(ProxyBase).GetMethod("Call",BindingFlags.NonPublic|BindingFlags.Instance);
			}

			gen.Emit(OpCodes.Ldarg_0); // this
			gen.Emit(OpCodes.Ldstr,methodInfo.Name);

			if(parameters.Length==0) 
			{
				// As Instance is static we load against null
				var @void=typeof(VoidValue).GetField("Instance",BindingFlags.Public|BindingFlags.Static);
				gen.Emit(OpCodes.Ldnull);
				gen.Emit(OpCodes.Ldfld,@void);
			}
			else
			{				
				gen.Emit(OpCodes.Ldarg_1);
				
				// Box, if required
				if(parameterType.IsValueType) gen.Emit(OpCodes.Box,parameterType);
			}

			gen.Emit(OpCodes.Callvirt,target);
			gen.Emit(OpCodes.Ret);

			builder.DefineMethodOverride(method,methodInfo);
		}

		private static TypeBuilder CreateTypeBuilder(string typeName, Type @interface)
		{
			var builder=AssemblyControl.ModuleBuilder.DefineType(typeName,TypeAttributes.Public,typeof(ProxyBase));
			builder.AddInterfaceImplementation(@interface);

			return builder;
		}

		private static Type ExtractMessageProtocol(Type type)
		{
			var attributes=type.GetCustomAttributes(typeof(ChurchServiceAttribute),true);
			if(attributes==null || attributes.Length==0) throw new InvalidOperationException("could not find ChurchService attribute");

			var churchService=(ChurchServiceAttribute)attributes[0];
			return churchService.MessageProtocolType;
		}

		private static string GenerateClassName(Type @interface)
		{
			var id=Interlocked.Increment(ref s_ClassID);
			string name=string.Format("Impl_{0}_{1}",@interface.Name,id);

			return name;
		}

		private static Type ExtractTaskReturnType(Type type)
		{
			if(typeof(Task).IsAssignableFrom(type)==false) throw new ArgumentException("return type is not a task");

			if(type.IsGenericType)
			{
				return type.GenericTypeArguments[0];
			}
			else
			{
				return typeof(void);
			}
		}
	}
}
