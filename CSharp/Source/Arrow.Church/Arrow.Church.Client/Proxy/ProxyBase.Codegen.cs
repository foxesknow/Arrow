using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;
using Arrow.Church.Common.Data;

namespace Arrow.Church.Client.Proxy
{
	partial class ProxyBase
	{
		public static Type GenerateProxy(Type @interface)
		{
			var builder=CreateTypeBuilder("foo",@interface);

			ImplementConstructor(builder);

			foreach(var methodInfo in @interface.GetMethods())
			{
				ImplementMethod(builder,methodInfo);
			}

			Type type=builder.CreateType();
			return type;
		}

		private static void ImplementConstructor(TypeBuilder builder)
		{
			var ctorParams=new Type[]{typeof(Uri),typeof(ServiceDispatcher)};
			ConstructorInfo baseCtor=typeof(ProxyBase).GetConstructor(ctorParams);

			MethodAttributes attr=MethodAttributes.Public|MethodAttributes.HideBySig;
			ConstructorBuilder ctor=builder.DefineConstructor(attr,CallingConventions.Standard,ctorParams);

			var gen=ctor.GetILGenerator();
			gen.Emit(OpCodes.Ldarg_0); // this
			gen.Emit(OpCodes.Ldarg_1); // Uri
			gen.Emit(OpCodes.Ldarg_2); // ServiceDispatcher
			gen.Emit(OpCodes.Call,baseCtor);
			gen.Emit(OpCodes.Ret);
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
	}
}
