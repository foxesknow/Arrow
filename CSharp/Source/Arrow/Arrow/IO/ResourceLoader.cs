using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Arrow.IO
{
	/// <summary>
	/// Useful resource methods
	/// </summary>
	public static class ResourceLoader
	{
		/// <summary>
		/// Returns a stream to a resource in an assembly
		/// </summary>
		/// <param name="assembly">The assembly to load from</param>
		/// <param name="filename">The fully qualified filename of the resource to load</param>
		/// <returns>A stream to the resource</returns>
		public static Stream Open(Assembly assembly, string filename)
		{
			if(assembly==null) throw new ArgumentNullException("assembly");
			if(filename==null) throw new ArgumentNullException("filename");

			// Be forgiving and map \ and / 
			string adjustedPath=filename.Replace('/','.');
			adjustedPath=filename.Replace('\\','.');

			return assembly.GetManifestResourceStream(adjustedPath)!;
		}

		/// <summary>
		/// Returns a stream to a resource in an assembly
		/// </summary>
		/// <typeparam name="T">The type whos assembly we'll use</typeparam>
		/// <param name="filename">The fully qualified filename of the resource to load</param>
		/// <returns>A stream to the resource</returns>
		public static Stream Open<T>(string filename)
		{
			return Open(typeof(T).Assembly,filename);
		}
	}
}
