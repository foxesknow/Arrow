using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace Arrow.Settings
{
	/// <summary>
	/// Settings for the current process
	/// </summary>
	/// <remarks>
	/// The settings available are:
	/// 
	///		exe				The name of the process executable
	///		pid				The process id
	///		version			The application version
	///		AssemblyName	The assembly name of the process
	///		StartTime		The time the process was started (DateTime)
	///		StartupDir		The directory the application was launched from
	///		this			The Process object for the current app
	/// 
	/// </remarks>
	public class ProcessSettings : ISettings
	{
		/// <summary>
		/// An instance that may be shared
		/// </summary>		
		public static readonly ISettings Instance=new ProcessSettings();

		/// <summary>
		/// Retrives a process setting.
		/// </summary>
		/// <param name="name">The process setting name</param>
		/// <returns>The process setting</returns>
		public object GetSetting(string name)
		{
			switch(name.ToLower())
			{
				case "exe":
					return GetProcessName();
					
				case "pid":
					return Process.GetCurrentProcess().Id.ToString();
					
				case "version":
					return GetAssemblyName().Version.ToString();
					
				case "assemblyname":
					return GetAssemblyName().Name;
					
				case "starttime":
					return Process.GetCurrentProcess().StartTime;
					
				case "startupdir":
					string dir=AppDomain.CurrentDomain.BaseDirectory;
					if(dir.EndsWith("\\")) dir=dir.Substring(0,dir.Length-1);
					return dir;
					
				case "this":
					return Process.GetCurrentProcess();
					
				default:
					return null;
			}
		}

		/// <summary>
		/// Determines the name of the active process
		/// </summary>
		/// <returns>The name of the process</returns>
		private string GetProcessName()
		{
			string process=null;
			
			try
			{
				Assembly assembly=Assembly.GetEntryAssembly();
				if(!(assembly is System.Reflection.Emit.AssemblyBuilder))
				{
					if(IsAssemblyInGAC(assembly)==false)
					{
						FileInfo info=new FileInfo(assembly.Location);
						process=info.Name;
						if(process.ToLower().EndsWith(".exe"))
						{
							process=process.Substring(0,process.Length-4);
						}
					}
				}
			}
			catch(Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
				throw;
			}
			
			return process ?? "unknown";
		}
		
		/// <summary>
		/// Determines the name of the entry assembly
		/// </summary>
		/// <returns>The name of the entry assembly</returns>
		private AssemblyName GetAssemblyName()
		{
			try
			{
				Assembly assembly=Assembly.GetEntryAssembly();
				if(!(assembly is System.Reflection.Emit.AssemblyBuilder))
				{
					if(IsAssemblyInGAC(assembly)==false)
					{
						return assembly.GetName();
					}
				}
			}
			catch(Exception e)
			{
				System.Diagnostics.Trace.WriteLine(e.Message);
				throw;
			}
			
			return null;
		}

		private bool IsAssemblyInGAC(Assembly assembly)
		{
			#if NETFRAMEWORK
				return assembly.GlobalAssemblyCache;
			#else
				return false;
			#endif
		}
	}
}
