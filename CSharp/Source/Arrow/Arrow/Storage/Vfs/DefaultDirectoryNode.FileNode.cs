using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	public partial class DefaultDirectoryNode
	{
		/// <summary>
		/// The file node class used by the default directory
		/// </summary>
		class DefaultFileNode : IFileNode
		{
			private readonly Func<Stream> m_File;

			public DefaultFileNode(Func<Stream> file)
			{
				m_File=file;
			}
	
			public Stream Open()
			{
				return m_File();
			}
		}
	}
}
