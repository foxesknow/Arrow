using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arrow.Storage.Vfs
{
	public partial class VirtualFileSystem : IDirectoryNode
	{
		IList<string> IDirectoryNode.GetFiles()
		{
			return m_Root.GetFiles();
		}

		IList<string> IDirectoryNode.GetDirectories()
		{
			return m_Root.GetDirectories();
		}

		IDirectoryNode IDirectoryNode.CreateDirectory(string name)
		{
			return m_Root.CreateDirectory(name);
		}

		IFileNode IDirectoryNode.CreateFile(string name, Func<System.IO.Stream> file)
		{
			return m_Root.CreateFile(name,file);
		}

		LookupResult IDirectoryNode.TryGetDirectory(string name, out IDirectoryNode directory)
		{
			return m_Root.TryGetDirectory(name,out directory);
		}

		LookupResult IDirectoryNode.TryGetFile(string name, out IFileNode file)
		{
			return m_Root.TryGetFile(name,out file);
		}

		bool IDirectoryNode.Delete(string name)
		{
			return m_Root.Delete(name);
		}

		bool IDirectoryNode.RegisterMount(string name, IDirectoryNode mountPoint)
		{
			return m_Root.RegisterMount(name,mountPoint);
		}

		LookupResult IDirectoryNode.TryGetMountPoint(string name, out IDirectoryNode mountPoint)
		{
			return m_Root.TryGetMountPoint(name,out mountPoint);
		}

		private IReadOnlyList<string> ToList(string name)
		{
			return new string[]{name};
		}
	}
}
