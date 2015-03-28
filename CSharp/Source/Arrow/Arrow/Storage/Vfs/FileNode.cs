using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	class FileNode : IFileNode
	{
		private readonly Func<Stream> m_File;

		public FileNode(Func<Stream> file)
		{
			m_File=file;
		}

		public bool IsFile
		{
			get{return true;}
		}

		public bool IsDirectory
		{
			get{return false;}
		}

		public Stream Open()
		{
			return m_File();
		}
	}
}
