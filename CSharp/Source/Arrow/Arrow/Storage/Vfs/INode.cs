using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	/// <summary>
	/// Base interface for items in the virtual file system
	/// </summary>
	public interface INode
	{
		/// <summary>
		/// Indicates if the node represents a file
		/// </summary>
		bool IsFile{get;}
		
		/// <summary>
		/// Indicates if the node represents a directory
		/// </summary>
		bool IsDirectory{get;}		
	}
}
