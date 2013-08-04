using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Arrow.Storage
{
	/// <summary>
	/// Accesses data from a file
	/// </summary>
	public class FileAccessor : Accessor
	{
		/// <summary>
		/// Initializes the instance
		/// </summary>
		/// <param name="uri">The uri of the file to access</param>
		public FileAccessor(Uri uri) : base(uri)
		{
			ValidateScheme(uri,Uri.UriSchemeFile);
		}

		/// <summary>
		/// Opens the filesystem file at the specified uri
		/// </summary>
		/// <returns>A stream to the file.</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		/// <exception cref="System.IO.IOException">The resource could not be opened</exception>
		/// <exception cref="System.IO.IOException">The uri does not use the file scheme</exception>
		public override Stream OpenRead()
		{
			string path=this.Uri.LocalPath;
			return File.OpenRead(path);
			
		}

		/// <summary>
		/// Always returns true
		/// </summary>
		public override bool CanExists
		{
			get{return true;}
		}

		/// <summary>
		/// Determines if a file exists
		/// </summary>
		/// <returns>true if the file exists, otherwise false</returns>
		/// <exception cref="System.ArgumentNullException">uri is null</exception>
		public override bool Exists()
		{
			string path=this.Uri.LocalPath;
			return File.Exists(path);
		}
	}
}
