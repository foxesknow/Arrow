using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arrow.Church.Common;

namespace Arrow.Church.Server
{
	class ServiceData
	{
		private readonly ChurchService m_Service;
		private readonly Dictionary<string,MethodDetails> m_ServiceMethods=new Dictionary<string,MethodDetails>();
		private readonly HashSet<Type> m_Interfaces=new HashSet<Type>();

		public ServiceData(ChurchService service)
		{
			m_Service=service;
		}

		public ChurchService Service
		{
			get{return m_Service;}
		}

		public void AddMethod(string name, ServiceMethod method, Type returnType, Type parameterType)
		{
			var details=new MethodDetails()
			{
				Method=method,
				ReturnType=returnType,
				ParameterType=parameterType
			};

			m_ServiceMethods.Add(name,details);
		}

		public void AddInterface(Type type)
		{
			m_Interfaces.Add(type);
		}

		public bool HasInterface(Type type)
		{
			return m_Interfaces.Contains(type);
		}

		public bool TryGetReturnType(string methodName, out Type returnType)
		{
			MethodDetails details;
			if(m_ServiceMethods.TryGetValue(methodName,out details))
			{
				returnType=details.ReturnType;
				return true;
			}
			else
			{
				returnType=null;
				return false;
			}
		}

		public bool TryGetParameterType(string methodName, out Type parameterType)
		{
			MethodDetails details;
			if(m_ServiceMethods.TryGetValue(methodName,out details))
			{
				parameterType=details.ParameterType;
				return true;
			}
			else
			{
				parameterType=null;
				return false;
			}
		}

		public Task<object> Execute(string method, object argument)
		{
			MethodDetails details=null;

			if(m_ServiceMethods.TryGetValue(method,out details))
			{
				ServiceMethod serviceMethod=details.Method;
				return serviceMethod(m_Service,argument);
			}
			else
			{
				throw new ChurchException("could not find method: "+method);
			}
		}

		class MethodDetails
		{
			public ServiceMethod Method;
			public Type ReturnType;
			public Type ParameterType;
		}
	}
}
