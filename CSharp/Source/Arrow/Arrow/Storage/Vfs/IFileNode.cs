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
	public interface IFileNode : INode
	{
		/// <summary>
		/// Returns a stream to the file represented by the node
		/// </summary>
		/// <returns>A stream</returns>
		Stream Open();
	}
}
