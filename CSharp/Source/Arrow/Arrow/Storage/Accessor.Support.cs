using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arrow.Storage
{
	public abstract partial class Accessor
	{
		/// <summary>
		/// Creates a Uri by examining a possibly relative uri.
		/// If the uri is actually a complete Uri then this is returned, otherwise it is combined with the baseUri
		/// If baseUri is null then possibleRelativeUri is passed to CreateUri
		/// </summary>
		/// <param name="baseUri">The base to create against</param>
		/// <param name="possibleRelativeUri">A uri string that may be relative</param>
		/// <returns>A uri for the resource</returns>
		/// <exception cref="System.ArgumentNullException">possibleRelativeUri is null</exception>
		public static Uri ResolveRelative(Uri? baseUri, string possibleRelativeUri)
		{
			if(possibleRelativeUri==null) throw new ArgumentNullException("possibleRelativeUri");
			
			if(baseUri==null) return CreateUri(possibleRelativeUri);
			
			Uri? uri=null;
			
			if(possibleRelativeUri.Contains(Uri.SchemeDelimiter))
			{
				uri=new Uri(possibleRelativeUri);
			}
			else
			{
				uri=new Uri(baseUri,possibleRelativeUri);
			}
			
			return uri;
		}
		
		/// <summary>
		/// Creates a Uri for a file
		/// </summary>
		/// <param name="filename">The name of the file to create the uri for</param>
		/// <returns>A Uri for the file</returns>
		/// <exception cref="System.ArgumentNullException">filename is null</exception>
		public static Uri CreateFileUri(string filename)
		{
			if(filename==null) throw new ArgumentNullException("filename");
			
			FileInfo info=new FileInfo(filename);
			return new Uri(info.FullName);
		}
		
		/// <summary>
		/// Creates a Uri for a directory
		/// </summary>
		/// <param name="directory">The name of the directory to create the uri for</param>
		/// <returns>A Uri for the directory</returns>
		/// <exception cref="System.ArgumentNullException">directory is null</exception>
		public static Uri CreateDirectoryUri(string directory)
		{
			if(directory==null) throw new ArgumentNullException("directory");
			
			DirectoryInfo info=new DirectoryInfo(directory);
			string path=info.FullName;
			if(path.EndsWith("\\")==false) path+="\\";
			
			return new Uri(path);
		}
		
		/// <summary>
		/// Creates a Uri by analysing uriText to determine the correct uri form
		/// </summary>
		/// <param name="uriText">The uri to analyse</param>
		/// <returns>A Uri instance</returns>
		/// <exception cref="System.ArgumentNullException">uriText is null</exception>
		public static Uri CreateUri(string uriText)
		{
			if(uriText==null) throw new ArgumentNullException("uriText");
			
			Uri? uri=null;
			
			if(uriText.Contains(Uri.SchemeDelimiter))
			{
				// It's a fully qualified uri, so no need to do anything fancy
				uri=new Uri(uriText);
			}
			else
			{
				// Assume it's a file uri
				uri=CreateFileUri(uriText);
			}
			
			return uri;
		}
	}
}
