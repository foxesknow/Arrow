using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Arrow.Storage
{
	/// <summary>
	/// Reads data from an assembly resource.
	/// The url must be of the form: res//assembly/fully/qualified/name
	/// For example: res::/Arrow/Arrow/Storage/file.xml
	/// The fully qualified name is case sensitive. You can use / or . to separate the path
	/// </summary>
	public class ResourceAccessor : Accessor
	{
		/// <summary>
		/// The scheme used by this acccessor
		/// </summary>
		public static readonly string Scheme="res";

		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="uri">The use of the resource to access</param>
		public ResourceAccessor(Uri uri) : base(uri)
		{
			ValidateScheme(uri,Scheme);
		}

		/// <summary>
		/// Reads from a resource stream. 
		/// The host part specifies the assembly, the path specifies the path within the assembly
		/// </summary>
		/// <returns>A stream to the resource</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		/// <exception cref="System.IO.IOException">The resource could not be opened</exception>
		/// <exception cref="System.IO.IOException">The uri does not use the res scheme</exception>
		public override Stream OpenRead()
		{
			string? assemblyName=null;
			string? resourcePath=null;
			
			ExtractResourceInfo(this.Uri,out assemblyName,out resourcePath);
			
			Assembly? assembly=null;
			
			try
			{
				assembly=Assembly.Load(assemblyName);
			}
			catch(Exception e)
			{
				throw new IOException(e.Message,e);
			}
			
			Stream? stream=assembly.GetManifestResourceStream(resourcePath);
			if(stream==null)
			{
				throw new IOException("could not find "+resourcePath);
			}
			
			return stream;
		}

		/// <summary>
		/// Always returns true
		/// </summary>
		public override bool CanExists
		{
			get{return true;}
		}

		/// <summary>
		/// Determines if a resource exists
		/// </summary>
		/// <returns>true if the resource exists, otherwise false</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public override bool Exists()
		{
			string? assemblyName=null;
			string? resourcePath=null;
			
			ExtractResourceInfo(this.Uri,out assemblyName,out resourcePath);
			
			try
			{
				Assembly assembly=Assembly.Load(assemblyName);
				var info=assembly.GetManifestResourceInfo(resourcePath);
				return info!=null;
			}
			catch(Exception)
			{
				return false;
			}
		}
		
		private void ExtractResourceInfo(Uri uri, out string assemblyName, out string resourcePath)
		{
			assemblyName=uri.Host;
			resourcePath=uri.AbsolutePath;
			
			// Normalize the path
			if(resourcePath.StartsWith("/")) resourcePath=resourcePath.Substring(1);
			resourcePath=resourcePath.Replace('/','.');
			resourcePath=resourcePath.Replace('\\','.');
		}

		/// <summary>
		/// Create a uri to a resource
		/// </summary>
		/// <param name="assembly">The assembly containing the resource</param>
		/// <param name="absolutePath">The path within the assembly to the resource</param>
		/// <returns>A uri to the resource</returns>
		public static Uri CreateUri(Assembly assembly, string absolutePath)
		{
			if(assembly==null) throw new ArgumentNullException("assembly");
			if(absolutePath==null) throw new ArgumentNullException("absolutePath");

			// We just want the assembly name (ie no version etc)
			string host=assembly.GetName().Name!;

			UriBuilder builder=new UriBuilder();
			builder.Scheme=Scheme;
			builder.Host=host;
			builder.Path=absolutePath;

			return builder.Uri;
		}
	}
}
