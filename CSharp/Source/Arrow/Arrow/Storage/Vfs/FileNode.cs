using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// Base class for files within the virtual file system
	/// </summary>
	public abstract class FileNode : INode
	{
		/// <summary>
		/// Returns a stream to the file represented by the node
		/// </summary>
		/// <returns>A stream</returns>
		public abstract Stream Open();

		/// <summary>
		/// Always returns true
		/// </summary>
		public bool IsFile
		{
			get{return true;}
		}

		/// <summary>
		/// Always returns false
		/// </summary>
		public bool IsDirectory
		{
			get{return false;}
		}
	}
}
