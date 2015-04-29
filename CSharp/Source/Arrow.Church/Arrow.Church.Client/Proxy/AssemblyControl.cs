using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Church.Client.Proxy
{
	class AssemblyControl
	{
		private static AssemblyBuilder s_AssemblyBuilder;
		private static ModuleBuilder s_ModuleBuilder;
		
		static AssemblyControl()
		{
			AssemblyName assemblyName=CreateAssemblyName();
			s_AssemblyBuilder=AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName,AssemblyBuilderAccess.Run);
			s_ModuleBuilder=s_AssemblyBuilder.DefineDynamicModule(assemblyName.Name);
		}
		
		/// <summary>
		/// Returns the AssemblyBuilder instance that will contain all dynamically generated code
		/// </summary>
		public static AssemblyBuilder AssemblyBuilder
		{
			get{return s_AssemblyBuilder;}
		}
		
		/// <summary>
		/// Returns the ModuleBuilder instance that will contain all dynamically generated code
		/// </summary>
		public static ModuleBuilder ModuleBuilder
		{
			get{return s_ModuleBuilder;}
		}
		
		/// <summary>
		/// Creates the name of the assembly
		/// </summary>
		/// <returns></returns>
		private static AssemblyName CreateAssemblyName()
		{
			AssemblyName name=new AssemblyName();
			name.Name="Arrow.Church.Client.Generated";
			return name;
		}
	}
}
